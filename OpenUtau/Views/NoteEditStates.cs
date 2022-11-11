﻿using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using OpenUtau.App.Controls;
using OpenUtau.App.ViewModels;
using OpenUtau.Core;
using OpenUtau.Core.Util;
using OpenUtau.Core.Ustx;

namespace OpenUtau.App.Views {
    class KeyboardPlayState {
        private readonly TrackBackground element;
        private readonly PianoRollViewModel vm;
        private SineGen? sineGen;
        public KeyboardPlayState(TrackBackground element, PianoRollViewModel vm) {
            this.element = element;
            this.vm = vm;
        }
        public void Begin(IPointer pointer, Point point) {
            pointer.Capture(element);
            var tone = vm.NotesViewModel.PointToTone(point);
            sineGen = PlaybackManager.Inst.PlayTone(MusicMath.ToneToFreq(tone));
        }
        public void Update(IPointer pointer, Point point) {
            var tone = vm.NotesViewModel.PointToTone(point);
            if (sineGen != null) {
                sineGen.Freq = MusicMath.ToneToFreq(tone);
            }
        }
        public void End(IPointer pointer, Point point) {
            pointer.Capture(null);
            if (sineGen != null) {
                sineGen.Stop = true;
            }
        }
    }

    class NoteEditState {
        public virtual MouseButton MouseButton => MouseButton.Left;
        public readonly Canvas canvas;
        public readonly PianoRollViewModel vm;
        public Point startPoint;
        public IValueTip valueTip;
        protected virtual bool ShowValueTip => true;
        public NoteEditState(Canvas canvas, PianoRollViewModel vm, IValueTip valueTip) {
            this.canvas = canvas;
            this.vm = vm;
            this.valueTip = valueTip;
        }
        public virtual void Begin(IPointer pointer, Point point) {
            pointer.Capture(canvas);
            startPoint = point;
            DocManager.Inst.StartUndoGroup();
            if (ShowValueTip) {
                valueTip.ShowValueTip();
            }
        }
        public virtual void End(IPointer pointer, Point point) {
            pointer.Capture(null);
            DocManager.Inst.EndUndoGroup();
            if (ShowValueTip) {
                valueTip.HideValueTip();
            }
        }
        public virtual void Update(IPointer pointer, Point point) { }
        public static void Swap<T>(ref T a, ref T b) {
            T temp = a;
            a = b;
            b = temp;
        }
        public static double Lerp(Point p1, Point p2, double x) {
            double t = (x - p1.X) / (p2.X - p1.X);
            t = Math.Clamp(t, 0, 1);
            return p1.Y + t * (p2.Y - p1.Y);
        }
    }

    class NoteSelectionEditState : NoteEditState {
        public readonly Rectangle selectionBox;
        protected override bool ShowValueTip => false;
        private int startTick;
        private int startTone;
        public NoteSelectionEditState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            Rectangle selectionBox) : base(canvas, vm, valueTip) {
            this.selectionBox = selectionBox;
        }
        public override void Begin(IPointer pointer, Point point) {
            pointer.Capture(canvas);
            startPoint = point;
            selectionBox.IsVisible = true;
            var notesVm = vm.NotesViewModel;
            startTick = notesVm.PointToTick(point);
            startTone = notesVm.PointToTone(point);
        }
        public override void End(IPointer pointer, Point point) {
            pointer.Capture(null);
            selectionBox.IsVisible = false;
            var notesVm = vm.NotesViewModel;
            notesVm.CommitTempSelectNotes();
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            int tick = notesVm.PointToTick(point);
            int tone = notesVm.PointToTone(point);

            int minTick = Math.Min(tick, startTick);
            int maxTick = Math.Max(tick, startTick);
            notesVm.TickToLineTick(minTick, out int x0, out int _);
            notesVm.TickToLineTick(maxTick, out int _, out int x1);

            int y0 = Math.Min(tone, startTone) - 1;
            int y1 = Math.Max(tone, startTone);

            var leftTop = notesVm.TickToneToPoint(x0, y1);
            var Size = notesVm.TickToneToSize(x1 - x0, y1 - y0);
            Canvas.SetLeft(selectionBox, leftTop.X);
            Canvas.SetTop(selectionBox, leftTop.Y);
            selectionBox.Width = Size.Width + 1;
            selectionBox.Height = Size.Height;
            notesVm.TempSelectNotes(x0, x1, y0, y1);
        }
    }

    class NoteMoveEditState : NoteEditState {
        public readonly UNote note;
        private double xOffset;
        protected override bool ShowValueTip => false;
        public NoteMoveEditState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note) : base(canvas, vm, valueTip) {
            this.note = note;
            var notesVm = vm.NotesViewModel;
            if (!notesVm.Selection.Contains(note)) {
                notesVm.DeselectNotes();
                notesVm.SelectNote(note);
            }
        }
        public override void Begin(IPointer pointer, Point point) {
            base.Begin(pointer, point);
            var notesVm = vm.NotesViewModel;
            xOffset = point.X - notesVm.TickToneToPoint(note.position, 0).X;
        }
        public override void Update(IPointer pointer, Point point) {
            var delta = point - startPoint;
            if (Math.Abs(delta.X) + Math.Abs(delta.Y) < 4) {
                return;
            }
            var project = DocManager.Inst.Project;
            var notesVm = vm.NotesViewModel;
            var part = notesVm.Part;
            if (part == null) {
                return;
            }

            int deltaTone = notesVm.PointToTone(point) - note.tone;
            int minDeltaTone;
            int maxDeltaTone;
            var selectedNotes = notesVm.Selection.ToList();
            if (selectedNotes.Count > 0) {
                minDeltaTone = -selectedNotes.Select(p => p.tone).Min();
                maxDeltaTone = ViewConstants.MaxTone - 1 - selectedNotes.Select(p => p.tone).Max();
            } else {
                minDeltaTone = -note.tone;
                maxDeltaTone = ViewConstants.MaxTone - 1 - note.tone;
            }
            deltaTone = Math.Clamp(deltaTone, minDeltaTone, maxDeltaTone);

            int snapUnit = project.resolution * 4 / notesVm.SnapDiv;
            int newPos = notesVm.PointToTick(point - new Point(xOffset, 0));
            if (notesVm.IsSnapOn) {
                newPos = (int)Math.Floor((double)newPos / snapUnit) * snapUnit;
            }
            int deltaTick = newPos - note.position;
            int minDeltaTick;
            int maxDeltaTick;
            if (selectedNotes.Count > 0) {
                minDeltaTick = -selectedNotes.Select(n => n.position).Min();
                maxDeltaTick = part.Duration - selectedNotes.Select(n => n.End).Max();
            } else {
                minDeltaTick = -note.position;
                maxDeltaTick = part.Duration - note.End;
            }
            deltaTick = Math.Clamp(deltaTick, minDeltaTick, maxDeltaTick);

            if (deltaTone == 0 && deltaTick == 0) {
                return;
            }
            if (selectedNotes.Count == 0) {
                DocManager.Inst.ExecuteCmd(new MoveNoteCommand(
                    part, note, deltaTick, deltaTone));
            } else {
                DocManager.Inst.ExecuteCmd(new MoveNoteCommand(
                    part, selectedNotes, deltaTick, deltaTone));
            }
        }
    }

    class NoteDrawEditState : NoteEditState {
        private UNote? note;
        private SineGen? sineGen;
        private bool playTone;
        public NoteDrawEditState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            bool playTone) : base(canvas, vm, valueTip) {
            this.playTone = playTone;
        }
        public override void Begin(IPointer pointer, Point point) {
            base.Begin(pointer, point);
            note = vm.NotesViewModel.MaybeAddNote(point, false);
            if (note != null && playTone) {
                sineGen = PlaybackManager.Inst.PlayTone(MusicMath.ToneToFreq(note.tone));
            }
        }
        public override void Update(IPointer pointer, Point point) {
            if (note == null) {
                return;
            }
            var project = DocManager.Inst.Project;
            var notesVm = vm.NotesViewModel;
            int tone = notesVm.PointToTone(point);
            if (sineGen != null) {
                sineGen.Freq = MusicMath.ToneToFreq(tone);
            }
            int deltaTone = tone - note.tone;
            int snapUnit = project.resolution * 4 / notesVm.SnapDiv;
            int newEnd = notesVm.PointToTick(point);
            if (notesVm.IsSnapOn) {
                newEnd = (int)Math.Floor((double)newEnd / snapUnit + 1) * snapUnit;
            }
            int deltaDuration = newEnd - note.End;
            int minNoteTicks = notesVm.IsSnapOn ? snapUnit : 15;
            if (deltaDuration < 0) {
                int maxNegDelta = note.duration - minNoteTicks;
                if (notesVm.Selection.Count > 0) {
                    maxNegDelta = notesVm.Selection.Min(n => n.duration - minNoteTicks);
                }
                if (notesVm.IsSnapOn && snapUnit > 0) {
                    maxNegDelta = (int)Math.Floor((double)maxNegDelta / snapUnit) * snapUnit;
                }
                deltaDuration = Math.Max(deltaDuration, -maxNegDelta);
            }
            if (deltaTone != 0) {
                DocManager.Inst.ExecuteCmd(new MoveNoteCommand(notesVm.Part, note, 0, deltaTone));
            }
            if (deltaDuration != 0) {
                DocManager.Inst.ExecuteCmd(new ResizeNoteCommand(notesVm.Part, note, deltaDuration));
                if (NotePresets.Default.AutoVibratoToggle && note.duration >= NotePresets.Default.AutoVibratoNoteDuration) {
                    DocManager.Inst.ExecuteCmd(new VibratoLengthCommand(notesVm.Part, note, NotePresets.Default.DefaultVibrato.VibratoLength));
                } else {
                    DocManager.Inst.ExecuteCmd(new VibratoLengthCommand(notesVm.Part, note, 0));
                }
            }
            valueTip.UpdateValueTip(note.duration.ToString());
        }
        public override void End(IPointer pointer, Point point) {
            base.End(pointer, point);
            if (sineGen != null) {
                sineGen.Stop = true;
            }
        }
    }

    class NoteResizeEditState : NoteEditState {
        public readonly UNote note;
        public readonly UNote? nextNote;
        public readonly bool resizeNext;
        public NoteResizeEditState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note,
            bool resizeNext) : base(canvas, vm, valueTip) {
            this.note = note;
            var notesVm = vm.NotesViewModel;
            if (!notesVm.Selection.Contains(note)) {
                notesVm.DeselectNotes();
            }
            this.resizeNext = notesVm.Selection.Count == 0 &&
                resizeNext && note.Next != null && note.End == note.Next.position;
            nextNote = note.Next;
        }
        public override void Update(IPointer pointer, Point point) {
            var project = DocManager.Inst.Project;
            var notesVm = vm.NotesViewModel;
            int snapUnit = project.resolution * 4 / notesVm.SnapDiv;
            int newEnd = notesVm.PointToTick(point);
            if (notesVm.IsSnapOn) {
                newEnd = (int)Math.Floor((double)newEnd / snapUnit) * snapUnit + snapUnit;
            }
            int deltaDuration = newEnd - note.End;
            int minNoteTicks = notesVm.IsSnapOn ? snapUnit : 15;
            if (deltaDuration < 0) {
                int maxNegDelta = note.duration - minNoteTicks;
                if (notesVm.Selection.Count > 0) {
                    maxNegDelta = notesVm.Selection.Min(n => n.duration - minNoteTicks);
                }
                if (notesVm.IsSnapOn && snapUnit > 0) {
                    maxNegDelta = (int)Math.Floor((double)maxNegDelta / snapUnit) * snapUnit;
                }
                deltaDuration = Math.Max(deltaDuration, -maxNegDelta);
            }
            if (resizeNext && nextNote != null) {
                var maxDelta = Math.Max(0, nextNote.duration - minNoteTicks);
                deltaDuration = Math.Min(deltaDuration, maxDelta);
            }
            if (deltaDuration == 0) {
                valueTip.UpdateValueTip(note.duration.ToString());
                return;
            }
            if (notesVm.Selection.Count == 0) {
                if (resizeNext) {
                    DocManager.Inst.ExecuteCmd(new MoveNoteCommand(notesVm.Part, nextNote, deltaDuration, 0));
                    DocManager.Inst.ExecuteCmd(new ResizeNoteCommand(notesVm.Part, nextNote, -deltaDuration));
                }
                DocManager.Inst.ExecuteCmd(new ResizeNoteCommand(notesVm.Part, note, deltaDuration));
                valueTip.UpdateValueTip(note.duration.ToString());
                return;
            }
            DocManager.Inst.ExecuteCmd(new ResizeNoteCommand(notesVm.Part, notesVm.Selection.ToList(), deltaDuration));
            valueTip.UpdateValueTip(note.duration.ToString());
        }
    }

    class NoteSplitEditState : NoteEditState {
        public readonly UNote note;
        private UNote? newNote;
        private int oldDur;
        private float oldVibLength;
        private float oldVibFadeIn;
        private float oldVibFadeOut;
        private float oldVibShift;
        private float oldVibLengthTicks => oldVibLength * oldDur / 100;
        private float oldVibFadeInTicks => oldVibFadeIn * oldVibLengthTicks / 100;
        private float oldVibFadeOutTicks => oldVibFadeOut * oldVibLengthTicks / 100;
        private float vibPeriod => note.vibrato.period;
        public NoteSplitEditState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note) : base(canvas, vm, valueTip) {
            this.note = note;
            var notesVm = vm.NotesViewModel;
            if (!notesVm.Selection.Contains(note)) {
                notesVm.DeselectNotes();
            }
            oldDur = note.duration;
            oldVibLength = note.vibrato.length;
            oldVibFadeIn = note.vibrato.@in;
            oldVibFadeOut = note.vibrato.@out;
            oldVibShift = note.vibrato.shift;
        }

        public override void Begin(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            base.Begin(pointer, point);
            newNote = notesVm.MaybeAddNote(point, false);
            DocManager.Inst.ExecuteCmd(new ChangeNoteLyricCommand(notesVm.Part, newNote, "+"));
        }

        public override void Update(IPointer pointer, Point point) {
            var project = DocManager.Inst.Project;
            var notesVm = vm.NotesViewModel;
            int snapUnit = project.resolution * 4 / notesVm.SnapDiv;
            int tick = notesVm.PointToTick(point);
            int roundedSnappedTick = (int)Math.Round((double)tick / snapUnit) * snapUnit;
            int deltaDuration = notesVm.IsSnapOn
                ? roundedSnappedTick - note.End
                : tick - note.End;
            int minNoteTicks = notesVm.IsSnapOn ? snapUnit : 15;

            int maxNegDelta = note.duration - minNoteTicks;
            if (notesVm.IsSnapOn && snapUnit > 0) {
                maxNegDelta = (int)Math.Floor((double)maxNegDelta / snapUnit) * snapUnit;
            }
            deltaDuration = Math.Max(deltaDuration, -maxNegDelta);

            if (deltaDuration == 0) {
                valueTip.UpdateValueTip(note.duration.ToString());
                return;
            }
            if (note.duration + deltaDuration < oldDur) {
                DocManager.Inst.ExecuteCmd(new ResizeNoteCommand(notesVm.Part, newNote, -deltaDuration));
                DocManager.Inst.ExecuteCmd(new ResizeNoteCommand(notesVm.Part, note, deltaDuration));
                if (note.duration > oldDur - 10) DocManager.Inst.ExecuteCmd(new ResizeNoteCommand(notesVm.Part, note, oldDur - note.duration - 10));
                if (note.duration + newNote.duration > oldDur) DocManager.Inst.ExecuteCmd(new ResizeNoteCommand(notesVm.Part, newNote, -(note.duration + newNote.duration - oldDur))); ;
                DocManager.Inst.ExecuteCmd(new MoveNoteCommand(notesVm.Part, newNote, note.End - newNote.position, 0));
            }

            if (oldVibLength > 0) {
                DocManager.Inst.ExecuteCmd(new VibratoDepthCommand(notesVm.Part, newNote, note.vibrato.depth));
                DocManager.Inst.ExecuteCmd(new VibratoPeriodCommand(notesVm.Part, newNote, note.vibrato.period));

                if (oldVibLengthTicks > newNote.duration) {
                    float newVibLengthTicks = oldVibLengthTicks - newNote.duration;
                    //length correction
                    DocManager.Inst.ExecuteCmd(new VibratoLengthCommand(notesVm.Part, newNote, 100));
                    DocManager.Inst.ExecuteCmd(new VibratoLengthCommand(notesVm.Part, note, newVibLengthTicks * 100 / note.duration));
                    //fade in/out correction
                    DocManager.Inst.ExecuteCmd(new VibratoFadeInCommand(notesVm.Part, note, oldVibFadeInTicks * 100 / newVibLengthTicks));
                    DocManager.Inst.ExecuteCmd(new VibratoFadeOutCommand(notesVm.Part, note, 0));
                    DocManager.Inst.ExecuteCmd(new VibratoFadeInCommand(notesVm.Part, newNote, 0));
                    DocManager.Inst.ExecuteCmd(new VibratoFadeOutCommand(notesVm.Part, newNote, oldVibFadeOutTicks * 100 / newNote.duration));
                    //phase correction
                    double newVibLengthMs = project.timeAxis.MsBetweenTickPos(newNote.position, newNote.position + newVibLengthTicks);
                    float newVibShift = (float)(100 * (newVibLengthMs % vibPeriod / vibPeriod)) + oldVibShift;
                    if (newVibShift > 100) newVibShift -= 100;
                    DocManager.Inst.ExecuteCmd(new VibratoShiftCommand(notesVm.Part, newNote, newVibShift));
                } else {
                    DocManager.Inst.ExecuteCmd(new VibratoLengthCommand(notesVm.Part, note, 0));
                    DocManager.Inst.ExecuteCmd(new VibratoLengthCommand(notesVm.Part, newNote, oldVibLengthTicks * 100 / newNote.duration));
                    DocManager.Inst.ExecuteCmd(new VibratoFadeInCommand(notesVm.Part, newNote, oldVibFadeIn));
                    DocManager.Inst.ExecuteCmd(new VibratoFadeOutCommand(notesVm.Part, newNote, oldVibFadeOut));
                    DocManager.Inst.ExecuteCmd(new VibratoShiftCommand(notesVm.Part, newNote, oldVibShift));
                }
            }

            valueTip.UpdateValueTip(note.duration.ToString());
        }
    }

    class NoteEraseEditState : NoteEditState {
        public override MouseButton MouseButton => mouseButton;
        private MouseButton mouseButton;
        protected override bool ShowValueTip => false;
        public NoteEraseEditState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            MouseButton mouseButton) : base(canvas, vm, valueTip) {
            this.mouseButton = mouseButton;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            var noteHitInfo = notesVm.HitTest.HitTestNote(point);
            if (noteHitInfo.hitBody) {
                DocManager.Inst.ExecuteCmd(new RemoveNoteCommand(notesVm.Part, noteHitInfo.note));
            }
        }
    }

    class NotePanningState : NoteEditState {
        public override MouseButton MouseButton => MouseButton.Middle;
        protected override bool ShowValueTip => false;
        public NotePanningState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip) : base(canvas, vm, valueTip) { }
        public override void Begin(IPointer pointer, Point point) {
            pointer.Capture(canvas);
            startPoint = point;
        }
        public override void End(IPointer pointer, Point point) {
            pointer.Capture(null);
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            double deltaX = (point.X - startPoint.X) / notesVm.TickWidth;
            double deltaY = (point.Y - startPoint.Y) / notesVm.TrackHeight;
            startPoint = point;
            notesVm.TickOffset = Math.Max(0, notesVm.TickOffset - deltaX);
            notesVm.TrackOffset = Math.Max(0, notesVm.TrackOffset - deltaY);
        }
    }

    class PitchPointEditState : NoteEditState {
        public readonly UNote note;
        private bool onPoint;
        private float x;
        private float y;
        private int index;
        private PitchPoint pitchPoint;
        public PitchPointEditState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note,
            int index, bool onPoint, float x, float y) : base(canvas, vm, valueTip) {
            this.note = note;
            this.index = index;
            this.onPoint = onPoint;
            this.x = x;
            this.y = y;
            pitchPoint = note.pitch.data[index];
        }
        public override void Begin(IPointer pointer, Point point) {
            base.Begin(pointer, point);
            if (!onPoint) {
                pitchPoint = new PitchPoint(x, y);
                index++;
                DocManager.Inst.ExecuteCmd(new AddPitchPointCommand(
                    vm.NotesViewModel.Part, note, pitchPoint, index));
            }
        }
        public override void End(IPointer pointer, Point point) {
            if (note.pitch.data.Count > 2) {
                var notesVm = vm.NotesViewModel;
                bool removed = false;
                if (index > 0 && index < note.pitch.data.Count - 1) {
                    var prev = note.pitch.data[index - 1];
                    var delta = notesVm.TickToneToSize(prev.X - pitchPoint.X, (prev.Y - pitchPoint.Y) * 0.1);
                    if (delta.Width * delta.Width + delta.Height * delta.Height < 64) {
                        DocManager.Inst.ExecuteCmd(new DeletePitchPointCommand(notesVm.Part, note, index));
                        removed = true;
                    }
                    if (!removed) {
                        var next = note.pitch.data[index + 1];
                        delta = notesVm.TickToneToSize(next.X - pitchPoint.X, (next.Y - pitchPoint.Y) * 0.1);
                        if (delta.Width * delta.Width + delta.Height * delta.Height < 64) {
                            DocManager.Inst.ExecuteCmd(new DeletePitchPointCommand(notesVm.Part, note, index));
                        }
                    }
                }
            }
            base.End(pointer, point);
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            int partPos = notesVm.Part?.position ?? 0;
            double x = notesVm.Project.timeAxis.TickPosToMsPos(notesVm.PointToTick(point) + partPos);
            double deltaX = x - (note.PositionMs + pitchPoint.X);
            bool isFirst = index == 0;
            bool isLast = index == note.pitch.data.Count - 1;
            if (!isFirst) {
                deltaX = Math.Max(deltaX, note.pitch.data[index - 1].X - pitchPoint.X);
            }
            if (!isLast) {
                deltaX = Math.Min(deltaX, note.pitch.data[index + 1].X - pitchPoint.X);
            }
            double deltaY;
            if (isLast) {
                deltaY = -pitchPoint.Y;
            } else if (isFirst && note.pitch.snapFirst) {
                var snapTo = note.Prev == null ? note : note.Prev.End == note.position ? note.Prev : note;
                deltaY = (snapTo.tone - note.tone) * 10 - pitchPoint.Y;
            } else {
                deltaY = (notesVm.PointToToneDouble(point) - note.tone) * 10 - pitchPoint.Y;
            }
            if (deltaX == 0 && deltaY == 0) {
                return;
            }
            DocManager.Inst.ExecuteCmd(new MovePitchPointCommand(notesVm.Part, pitchPoint, (float)deltaX, (float)deltaY));
            valueTip.UpdateValueTip($"{pitchPoint.X:0.0}ms, {pitchPoint.Y * 10:0}cent");
        }
    }

    class ExpSetValueState : NoteEditState {
        private Point lastPoint;
        private UExpressionDescriptor? descriptor;
        private UTrack track;
        public ExpSetValueState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip) : base(canvas, vm, valueTip) {
            var notesVm = vm.NotesViewModel;
            var project = notesVm.Project;
            var part = notesVm.Part;
            track = project.tracks[part!.trackNo];
            if (project == null || part == null ||
                !track.TryGetExpression(
                    project, notesVm.PrimaryKey, out descriptor)) {
                descriptor = null;
            }
        }
        public override void Begin(IPointer pointer, Point point) {
            base.Begin(pointer, point);
            lastPoint = point;
        }
        public override void End(IPointer pointer, Point point) {
            base.End(pointer, point);
        }
        public override void Update(IPointer pointer, Point point) {
            if (descriptor == null) {
                return;
            }
            if (descriptor.type != UExpressionType.Curve) {
                UpdatePhonemeExp(pointer, point);
            } else {
                UpdateCurveExp(pointer, point);
            }
            double viewMax = descriptor.max + (descriptor.type == UExpressionType.Options ? 1 : 0);
            double displayValue = descriptor.min + (viewMax - descriptor.min) * (1 - point.Y / canvas.Bounds.Height);
            displayValue = Math.Max(descriptor.min, Math.Min(descriptor.max, displayValue));
            string valueTipText;
            if (descriptor.type == UExpressionType.Options) {
                int index = (int)displayValue;
                if (index >= 0 && index < descriptor.options.Length) {
                    valueTipText = descriptor.options[index];
                } else {
                    valueTipText = "Error: out of range";
                }
                if (string.IsNullOrEmpty(valueTipText)) {
                    valueTipText = "\"\"";
                }
            } else {
                valueTipText = ((int)displayValue).ToString();
            }
            valueTip.UpdateValueTip(valueTipText);
            lastPoint = point;
        }
        private void UpdatePhonemeExp(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            var p1 = lastPoint;
            var p2 = point;
            if (p1.X > p2.X) {
                Swap(ref p1, ref p2);
            }
            string key = notesVm.PrimaryKey;
            var hits = notesVm.HitTest.HitTestExpRange(p1, p2);
            double viewMax = descriptor.max + (descriptor.type == UExpressionType.Options ? 1 : 0);
            foreach (var hit in hits) {
                var valuePoint = notesVm.TickToneToPoint(hit.note.position + hit.phoneme.position, 0);
                double y = Lerp(p1, p2, valuePoint.X);
                double newValue = descriptor.min + (viewMax - descriptor.min) * (1 - y / canvas.Bounds.Height);
                newValue = Math.Max(descriptor.min, Math.Min(descriptor.max, newValue));
                float value = hit.phoneme.GetExpression(notesVm.Project, track, key).Item1;
                if ((int)newValue != (int)value) {
                    DocManager.Inst.ExecuteCmd(new SetPhonemeExpressionCommand(
                        notesVm.Project, track, notesVm.Part, hit.phoneme, key, (int)newValue));
                }
            }
        }
        private void UpdateCurveExp(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            int lastX = notesVm.PointToTick(lastPoint);
            int x = notesVm.PointToTick(point);
            int lastY = (int)Math.Round(descriptor.min + (descriptor.max - descriptor.min) * (1 - lastPoint.Y / canvas.Bounds.Height));
            int y = (int)Math.Round(descriptor.min + (descriptor.max - descriptor.min) * (1 - point.Y / canvas.Bounds.Height));
            DocManager.Inst.ExecuteCmd(new SetCurveCommand(notesVm.Project, notesVm.Part, notesVm.PrimaryKey, x, y, lastX, lastY));
        }
    }

    class ExpResetValueState : NoteEditState {
        private Point lastPoint;
        private UExpressionDescriptor? descriptor;
        private UTrack track;
        public override MouseButton MouseButton => MouseButton.Right;
        public ExpResetValueState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip) : base(canvas, vm, valueTip) {
            var notesVm = vm.NotesViewModel;
            var project = notesVm.Project;
            var part = notesVm.Part;
            track = project.tracks[part!.trackNo];
            if (project == null || part == null ||
                !track.TryGetExpression(
                    project, notesVm.PrimaryKey, out descriptor)) {
                descriptor = null;
            }
        }
        public override void Begin(IPointer pointer, Point point) {
            base.Begin(pointer, point);
            lastPoint = point;
        }
        public override void Update(IPointer pointer, Point point) {
            if (descriptor == null) {
                return;
            }
            if (descriptor.type != UExpressionType.Curve) {
                ResetPhonemeExp(pointer, point);
            } else {
                ResetCurveExp(pointer, point);
            }
            valueTip.UpdateValueTip(descriptor.defaultValue.ToString());
        }
        private void ResetPhonemeExp(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            var p1 = lastPoint;
            var p2 = point;
            if (p1.X > p2.X) {
                Swap(ref p1, ref p2);
            }
            string key = notesVm.PrimaryKey;
            var hits = notesVm.HitTest.HitTestExpRange(p1, p2);
            foreach (var hit in hits) {
                float value = hit.phoneme.GetExpression(notesVm.Project, track, key).Item1;
                if (value != descriptor.defaultValue) {
                    DocManager.Inst.ExecuteCmd(new SetPhonemeExpressionCommand(
                        notesVm.Project, track, notesVm.Part, hit.phoneme, key, descriptor.defaultValue));
                }
            }
        }
        private void ResetCurveExp(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            int lastX = notesVm.PointToTick(lastPoint);
            int x = notesVm.PointToTick(point);
            DocManager.Inst.ExecuteCmd(new SetCurveCommand(
                notesVm.Project, notesVm.Part, notesVm.PrimaryKey,
                x, (int)descriptor.defaultValue, lastX, (int)descriptor.defaultValue));
        }
    }

    class VibratoChangeStartState : NoteEditState {
        public readonly UNote note;
        public VibratoChangeStartState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note) : base(canvas, vm, valueTip) {
            this.note = note;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            int tick = notesVm.PointToTick(point);
            float newLength = 100f - 100f * (tick - note.position) / note.duration;
            if (newLength != note.vibrato.length) {
                DocManager.Inst.ExecuteCmd(new VibratoLengthCommand(notesVm.Part, note, newLength));
            }
            valueTip.UpdateValueTip($"{note.vibrato.length:0}%");
        }
    }

    class VibratoChangeInState : NoteEditState {
        public readonly UNote note;
        public VibratoChangeInState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note) : base(canvas, vm, valueTip) {
            this.note = note;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            int tick = notesVm.PointToTick(point);
            float vibratoTick = note.vibrato.length / 100f * note.duration;
            float startTick = note.position + note.duration - vibratoTick;
            float newIn = (tick - startTick) / vibratoTick * 100f;
            if (newIn != note.vibrato.@in) {
                if (newIn + note.vibrato.@out > 100) {
                    DocManager.Inst.ExecuteCmd(new VibratoFadeOutCommand(notesVm.Part, note, 100 - newIn));
                }
                DocManager.Inst.ExecuteCmd(new VibratoFadeInCommand(notesVm.Part, note, newIn));
            }
            valueTip.UpdateValueTip($"{note.vibrato.@in:0}%");
        }
    }

    class VibratoChangeOutState : NoteEditState {
        public readonly UNote note;
        public VibratoChangeOutState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note) : base(canvas, vm, valueTip) {
            this.note = note;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            int tick = notesVm.PointToTick(point);
            float vibratoTick = note.vibrato.length / 100f * note.duration;
            float newOut = (note.position + note.duration - tick) / vibratoTick * 100f;
            if (newOut != note.vibrato.@out) {
                if (newOut + note.vibrato.@in > 100) {
                    DocManager.Inst.ExecuteCmd(new VibratoFadeInCommand(notesVm.Part, note, 100 - newOut));
                }
                DocManager.Inst.ExecuteCmd(new VibratoFadeOutCommand(notesVm.Part, note, newOut));
            }
            valueTip.UpdateValueTip($"{note.vibrato.@out:0}%");
        }
    }

    class VibratoChangeDepthState : NoteEditState {
        public readonly UNote note;
        public VibratoChangeDepthState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note) : base(canvas, vm, valueTip) {
            this.note = note;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            float tone = (float)notesVm.PointToToneDouble(point) - 0.5f;
            float newDepth = note.vibrato.ToneToDepth(note, tone);
            if (newDepth != note.vibrato.depth) {
                DocManager.Inst.ExecuteCmd(new VibratoDepthCommand(notesVm.Part, note, newDepth));
            }
            valueTip.UpdateValueTip($"{note.vibrato.depth:0.0}");
        }
    }

    class VibratoChangePeriodState : NoteEditState {
        public readonly UNote note;
        public VibratoChangePeriodState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note) : base(canvas, vm, valueTip) {
            this.note = note;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            var project = notesVm.Project;
            int partPos = notesVm.Part?.position ?? 0;
            float vibratoTick = note.vibrato.length / 100f * note.duration;
            float startTick = note.position + note.duration - vibratoTick;
            double startMs = project.timeAxis.TickPosToMsPos(startTick + partPos);
            double pointerMs = project.timeAxis.TickPosToMsPos(notesVm.PointToTick(point) + notesVm.Part.position);
            float newPeriod = (float)((pointerMs - startMs) / (1 + note.vibrato.shift / 100f));
            if (newPeriod != note.vibrato.period) {
                DocManager.Inst.ExecuteCmd(new VibratoPeriodCommand(notesVm.Part, note, newPeriod));
            }
            valueTip.UpdateValueTip($"{note.vibrato.period:0.0}ms");
        }
    }

    class VibratoChangeShiftState : NoteEditState {
        public readonly UNote note;
        public readonly Point hitPoint;
        public readonly float initialShift;
        public VibratoChangeShiftState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote note,
            Point hitPoint,
            float initialShift) : base(canvas, vm, valueTip) {
            this.note = note;
            this.hitPoint = hitPoint;
            this.initialShift = initialShift;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            var project = notesVm.Project;
            float periodTick = project.timeAxis.TicksBetweenMsPos(note.PositionMs, note.PositionMs + note.vibrato.period);
            float deltaTick = notesVm.PointToTick(point) - notesVm.PointToTick(hitPoint);
            float deltaShift = deltaTick / periodTick * 100f;
            float newShift = initialShift + deltaShift;
            if (newShift != note.vibrato.shift) {
                DocManager.Inst.ExecuteCmd(new VibratoShiftCommand(notesVm.Part, note, newShift));
            }
            valueTip.UpdateValueTip($"{note.vibrato.shift:0}%");
        }
    }

    class PhonemeMoveState : NoteEditState {
        public readonly UNote leadingNote;
        public readonly UPhoneme phoneme;
        public readonly int index;
        public int startOffset;
        public PhonemeMoveState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote leadingNote,
            UPhoneme phoneme,
            int index) : base(canvas, vm, valueTip) {
            this.leadingNote = leadingNote;
            this.phoneme = phoneme;
            this.index = index;
        }
        public override void Begin(IPointer pointer, Point point) {
            base.Begin(pointer, point);
            startOffset = leadingNote.GetPhonemeOverride(index).offset ?? 0;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            int partPos = notesVm.Part?.position ?? 0;
            int offset = startOffset + notesVm.PointToTick(point) - notesVm.PointToTick(startPoint);
            DocManager.Inst.ExecuteCmd(new PhonemeOffsetCommand(
                notesVm.Part, leadingNote, index, offset));
            var project = notesVm.Project;
            double offsetMs = project.timeAxis.TickPosToMsPos(phoneme.position + offset + partPos) - phoneme.PositionMs;
            valueTip.UpdateValueTip($"{offsetMs:0.0}ms");
        }
    }

    class PhonemeChangePreutterState : NoteEditState {
        public readonly UNote leadingNote;
        public readonly UPhoneme phoneme;
        public readonly int index;
        public PhonemeChangePreutterState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote leadingNote,
            UPhoneme phoneme,
            int index) : base(canvas, vm, valueTip) {
            this.leadingNote = leadingNote;
            this.phoneme = phoneme;
            this.index = index;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            var project = notesVm.Project;
            double preutter = project.timeAxis.MsBetweenTickPos(notesVm.PointToTick(point), phoneme.position);
            double preutterDelta = preutter - phoneme.autoPreutter;
            preutterDelta = Math.Max(-phoneme.oto.Preutter, preutterDelta);
            DocManager.Inst.ExecuteCmd(new PhonemePreutterCommand(notesVm.Part, leadingNote, index, (float)preutterDelta));
            valueTip.UpdateValueTip($"{phoneme.preutter:0.0}ms ({preutterDelta:+0.0;-0.0;0}ms)");
        }
    }

    class PhonemeChangeOverlapState : NoteEditState {
        public readonly UNote leadingNote;
        public readonly UPhoneme phoneme;
        public readonly int index;
        public PhonemeChangeOverlapState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip,
            UNote leadingNote,
            UPhoneme phoneme,
            int index) : base(canvas, vm, valueTip) {
            this.leadingNote = leadingNote;
            this.phoneme = phoneme;
            this.index = index;
        }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            var project = notesVm.Project;
            int partPos = notesVm.Part?.position ?? 0;
            double overlap = project.timeAxis.TickPosToMsPos(notesVm.PointToTick(point) + partPos) - (phoneme.PositionMs - phoneme.preutter);
            double overlapDelta = overlap - phoneme.autoOverlap;
            DocManager.Inst.ExecuteCmd(new PhonemeOverlapCommand(notesVm.Part, leadingNote, index, (float)overlapDelta));
            valueTip.UpdateValueTip($"{phoneme.overlap:0.0}ms ({overlapDelta:+0.0;-0.0;0}ms)");
        }
    }

    class PhonemeResetState : NoteEditState {
        public override MouseButton MouseButton => MouseButton.Right;
        protected override bool ShowValueTip => false;
        public PhonemeResetState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip) : base(canvas, vm, valueTip) { }
        public override void Update(IPointer pointer, Point point) {
            var notesVm = vm.NotesViewModel;
            var hitInfo = notesVm.HitTest.HitTestPhoneme(point);
            if (hitInfo.hit) {
                var phoneme = hitInfo.phoneme;
                var parent = phoneme.Parent;
                var leadingNote = parent.Extends ?? parent;
                int index = phoneme.index;
                if (hitInfo.hitPosition) {
                    DocManager.Inst.ExecuteCmd(new PhonemeOffsetCommand(notesVm.Part, leadingNote, index, 0));
                } else if (hitInfo.hitPreutter) {
                    DocManager.Inst.ExecuteCmd(new PhonemePreutterCommand(notesVm.Part, leadingNote, index, 0));
                } else if (hitInfo.hitOverlap) {
                    DocManager.Inst.ExecuteCmd(new PhonemeOverlapCommand(notesVm.Part, leadingNote, index, 0));
                }
                return;
            }
            var aliasHitInfo = notesVm.HitTest.HitTestAlias(point);
            if (aliasHitInfo.hit) {
                var phoneme = aliasHitInfo.phoneme;
                if (phoneme.rawPhoneme != phoneme.phoneme) {
                    var note = phoneme.Parent;
                    int index = phoneme.index;
                    DocManager.Inst.ExecuteCmd(
                        new ChangePhonemeAliasCommand(
                            notesVm.Part, note.Extends ?? note, index, null));
                }
            }
        }
    }

    class DrawPitchState : NoteEditState {
        protected override bool ShowValueTip => false;
        double? lastPitch;
        Point lastPoint;
        public DrawPitchState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip) : base(canvas, vm, valueTip) { }
        public override void Begin(IPointer pointer, Point point) {
            base.Begin(pointer, point);
            lastPoint = point;
        }
        public override void Update(IPointer pointer, Point point) {
            int tick = vm.NotesViewModel.PointToTick(point);
            var samplePoint = vm.NotesViewModel.TickToneToPoint(
                (int)Math.Round(tick / 5.0) * 5,
                vm.NotesViewModel.PointToToneDouble(point));
            double? pitch = vm.NotesViewModel.HitTest.SamplePitch(samplePoint);
            if (pitch == null) {
                return;
            }
            double tone = vm.NotesViewModel.PointToToneDouble(point);
            DocManager.Inst.ExecuteCmd(new SetCurveCommand(
                vm.NotesViewModel.Project,
                vm.NotesViewModel.Part,
                Core.Format.Ustx.PITD,
                vm.NotesViewModel.PointToTick(point),
                (int)Math.Round(tone * 100 - pitch.Value),
                vm.NotesViewModel.PointToTick(lastPitch == null ? point : lastPoint),
                (int)Math.Round(tone * 100 - (lastPitch ?? pitch.Value))));
            lastPitch = pitch;
            lastPoint = point;
        }
    }

    class ResetPitchState : NoteEditState {
        public override MouseButton MouseButton => MouseButton.Right;
        protected override bool ShowValueTip => false;
        Point lastPoint;
        public ResetPitchState(
            Canvas canvas,
            PianoRollViewModel vm,
            IValueTip valueTip) : base(canvas, vm, valueTip) { }
        public override void Begin(IPointer pointer, Point point) {
            base.Begin(pointer, point);
            lastPoint = point;
        }
        public override void Update(IPointer pointer, Point point) {
            DocManager.Inst.ExecuteCmd(new SetCurveCommand(
                vm.NotesViewModel.Project,
                vm.NotesViewModel.Part,
                Core.Format.Ustx.PITD,
                vm.NotesViewModel.PointToTick(point),
                0,
                vm.NotesViewModel.PointToTick(lastPoint),
                0));
            lastPoint = point;
        }
    }
}
