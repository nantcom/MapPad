using NantCom.MapPad.Core.GamePadMappers;
using NantCom.MapPad.Types;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;

namespace NantCom.MapPad.Core
{
    /// <summary>
    /// Facade class which handles everything regarding the GamePad Mapping
    /// </summary>
    public class MapPad
    {
        private List<GamePadMapper> _Mappers = new List<GamePadMapper>();

        /// <summary>
        /// Gets the _Mappers thath will be used to map the gamepad input
        /// </summary>
        /// <value>
        /// The toReturn._Mappers.
        /// </value>
        public List<GamePadMapper> Mappers
        {
            get
            {
                return _Mappers;
            }
        }

        private CancellationTokenSource _Cancel;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <returns>True if game pad is found and can be polled</returns>
        public bool Start()
        {
            if (_Mappers.Count == 0)
            {
                throw new InvalidOperationException("No Mappings Defined");
            }

            if (_Cancel != null)
            {
                _Cancel.Cancel();
            }
            _Cancel = new CancellationTokenSource();

            var pad = Gamepad.StartPolling(_Cancel.Token);
            if (pad == null)
            {
                return false;
            }

            Gamepad.DataReceived += (s, e) =>
            {
                foreach (var update in e.JoystickUpdate)
                {
                    foreach (var mapper in _Mappers)
                    {
                        mapper.Handle(update);
                    }
                }

            };

            return true;
        }

        /// <summary>
        /// Stops the Mapping
        /// </summary>
        public void Stop()
        {
            if (_Cancel != null)
            {
                _Cancel.Cancel();
            }
        }

        /// <summary>
        /// Creates the MapPad Instance with default settings
        /// </summary>
        /// <returns></returns>
        public static MapPad CreateFPSDefault()
        {
            MapPad toReturn = new MapPad();

            toReturn._Mappers = new List<GamePadMapper>();
            toReturn._Mappers.Add(new AxisToKeyMapper()
            {
                Range = GamePadMapper.UpperHalfAxisRange,
                SourceOffset = JoystickOffset.X,
                Target = ScanCode.KEY_D
            });
            toReturn._Mappers.Add(new AxisToKeyMapper()
            {
                Range = GamePadMapper.LowerHalfAxisRange,
                SourceOffset = JoystickOffset.X,
                Target = ScanCode.KEY_A
            });
            toReturn._Mappers.Add(new AxisToKeyMapper()
            {
                Range = GamePadMapper.UpperHalfAxisRange,
                SourceOffset = JoystickOffset.Y,
                Target = ScanCode.KEY_S
            });
            toReturn._Mappers.Add(new AxisToKeyMapper()
            {
                Range = GamePadMapper.LowerHalfAxisRange,
                SourceOffset = JoystickOffset.Y,
                Target = ScanCode.KEY_W
            });
            toReturn._Mappers.Add(new AxisToMouseXMapper()
            {
                Range = GamePadMapper.AxisRange,
                Speed = 10,
                SourceOffset = JoystickOffset.Z
            });
            toReturn._Mappers.Add(new AxisToMouseYMapper()
            {
                Range = GamePadMapper.AxisRange,
                Speed = 10,
                SourceOffset = JoystickOffset.RotationZ
            });
            toReturn._Mappers.Add(new AxisToMouseYMapper()
            {
                Range = GamePadMapper.AxisRange,
                Speed = 10,
                SourceOffset = JoystickOffset.RotationZ
            });
            toReturn._Mappers.Add(new DigitalButtonToMouseButtonMapper()
            {
                Target = MouseButtonId.Right,
                SourceOffset = JoystickOffset.Buttons8
            });
            toReturn._Mappers.Add(new DigitalButtonToMouseButtonMapper()
            {
                Target = MouseButtonId.Left,
                SourceOffset = JoystickOffset.Buttons9
            });
            toReturn._Mappers.Add(new DigitalButtonToKeyMapper()
            {
                OnValue = 9000,
                OffValue = -1,
                Target = ScanCode.RIGHT,
                SourceOffset = JoystickOffset.PointOfViewControllers0
            });
            toReturn._Mappers.Add(new DigitalButtonToKeyMapper()
            {
                OnValue = 27000,
                OffValue = -1,
                Target = ScanCode.LEFT,
                SourceOffset = JoystickOffset.PointOfViewControllers0
            });
            toReturn._Mappers.Add(new DigitalButtonToKeyMapper()
            {
                OnValue = 0,
                OffValue = -1,
                Target = ScanCode.UP,
                SourceOffset = JoystickOffset.PointOfViewControllers0
            });
            toReturn._Mappers.Add(new DigitalButtonToKeyMapper()
            {
                OnValue = 18000,
                OffValue = -1,
                Target = ScanCode.DOWN,
                SourceOffset = JoystickOffset.PointOfViewControllers0
            });


            return toReturn;
        }
    }
}
