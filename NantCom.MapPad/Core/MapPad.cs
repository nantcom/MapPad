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
    /// Class which handles the gamepad mapping
    /// </summary>
    public static class MapPad
    {
        private static List<GamePadMapper> _Mappers = new List<GamePadMapper>();

        /// <summary>
        /// Gets the _Mappers thath will be used to map the gamepad input
        /// </summary>
        /// <value>
        /// The toReturn._Mappers.
        /// </value>
        public static List<GamePadMapper> Mappers
        {
            get
            {
                return _Mappers;
            }
        }

        /// <summary>
        /// Gets or sets whether mapping is enabled
        /// </summary>
        /// <value>
        /// The is enabled.
        /// </value>
        public static bool IsEnabled { get; set; }

        /// <summary>
        /// Sets the profile.
        /// </summary>
        /// <param name="p">The p.</param>
        public static void SetProfile(MappingProfile p)
        {
            var mapper = new List<GamePadMapper>();

            if (p.MouseHorizontal.OffsetId != -1)
            {
                mapper.Add(new AxisToMouseXMapper()
                {
                    Range = new NantCom.MapPad.Types.InputRange(p.MouseHorizontal.OnValue, p.MouseHorizontal.OffValue),
                    SourceOffset = (JoystickOffset)p.MouseHorizontal.OffsetId,
                });
            }

            if (p.MouseVertical.OffsetId != -1)
            {
                mapper.Add(new AxisToMouseYMapper()
                {
                    Range = new NantCom.MapPad.Types.InputRange(p.MouseVertical.OnValue, p.MouseVertical.OffValue),
                    SourceOffset = (JoystickOffset)p.MouseVertical.OffsetId,
                });
            }

            if (p.MouseLeft.IsValid)
            {
                mapper.Add(new DigitalButtonToMouseButtonMapper()
                {
                    OnValue = p.MouseLeft.OnValue,
                    OffValue = p.MouseLeft.OffValue,
                    SourceOffset = (JoystickOffset)p.MouseLeft.OffsetId,
                    Target = (MouseButtonId)p.MouseLeft.MouseButton
                });
            }

            if (p.MouseRight.IsValid)
            {
                mapper.Add(new DigitalButtonToMouseButtonMapper()
                {
                    OnValue = p.MouseRight.OnValue,
                    OffValue = p.MouseRight.OffValue,
                    SourceOffset = (JoystickOffset)p.MouseRight.OffsetId,
                    Target = (MouseButtonId)p.MouseRight.MouseButton
                });
            }

            foreach (var item in p.Keys)
            {
                if (item.IsValid == false)
                {
                    continue;
                }
                mapper.Add(new DigitalButtonToKeyMapper()
                                   {
                                       OnValue = item.OnValue,
                                       OffValue = item.OffValue,
                                       SourceOffset = (JoystickOffset)item.OffsetId,
                                       Target = (ScanCode)item.Key
                                   });

            }

            MapPad.IsEnabled = false;

            _Mappers = mapper;

            MapPad.IsEnabled = true;
        }

        static MapPad()
        {
            Gamepad.DataReceived += (s, e) =>
            {
                if (MapPad.IsEnabled == false || _Mappers.Count == 0)
                {
                    return;
                }

                foreach (var update in e.JoystickUpdate)
                {
                    foreach (var mapper in _Mappers)
                    {
                        mapper.Handle(update);
                    }
                }

            };
        }
    }
}
