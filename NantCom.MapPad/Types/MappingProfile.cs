using NantCom.MapPad.Lib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NantCom.MapPad.Types
{
    public class KeyMapping : NotifyPropertyChangedBase
    {
        public bool IsAxisMap { get; set; }

        public int OnValue { get; set; }

        public int OffValue { get; set; }

        public int OffsetId { get; set; }

        /// <summary>
        /// Gets or sets the key target
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public int Key { get; set; }

        /// <summary>
        /// Gets or sets the mouse button target
        /// </summary>
        /// <value>
        /// The mouse button.
        /// </value>
        public int MouseButton { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get
            {
                return (this.Key != -1 || this.MouseButton != -1) &&
                       (this.OffsetId != -1);
            }
        }

        public KeyMapping()
        {
            this.Key = -1;
            this.OffsetId = -1;
            this.MouseButton = -1;
        }

    }

    /// <summary>
    /// Represents the mapping profile
    /// </summary>
    public class MappingProfile : NotifyPropertyChangedBase
    {
        /// <summary>
        /// Gets or sets the mouse horizontal mapping.
        /// </summary>
        /// <value>
        /// The mouse horizontal.
        /// </value>
        public KeyMapping MouseHorizontal { get; set; }

        /// <summary>
        /// Gets or sets the mouse vertical mapping.
        /// </summary>
        /// <value>
        /// The mouse vertical.
        /// </value>
        public KeyMapping MouseVertical { get; set; }

        public KeyMapping MouseLeft { get; set; }

        public KeyMapping MouseRight { get; set; }

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        [JsonProperty("Keys")]
        public List<KeyMapping> KeysStorage { get; set; }

        private ObservableCollection<KeyMapping> _Keys;
        [JsonIgnore]
        public ObservableCollection<KeyMapping> Keys
        {
            get
            {
                if (_Keys == null)
                {
                    if (this.KeysStorage == null)
                    {
                        _Keys = new ObservableCollection<KeyMapping>();
                    }
                    else
                    {
                        _Keys = new ObservableCollection<KeyMapping>(this.KeysStorage);
                    }

                    
                    _Keys.CollectionChanged += (s, e) =>
                    {
                        this.KeysStorage = _Keys.ToList();
                    };
                }

                return _Keys;
            }
        }

        /// <summary>
        /// Gets or sets the name of the device that maps to
        /// </summary>
        /// <value>
        /// The name of the device.
        /// </value>
        public string DeviceName { get; set; }

        /// <summary>
        /// Gets or sets the name of the profile.
        /// </summary>
        /// <value>
        /// The name of the profile.
        /// </value>

        private string _ProfileName;

        ///<summary>
        ///Get or set the value of ProfileName
        ///</summary>
        public string ProfileName
        {
            get
            {
                return _ProfileName;
            }
            set
            {
                _ProfileName = value;
                this.OnPropertyChanged("ProfileName");
            }
        }
				
			

        /// <summary>
        /// Gets or sets the profile image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public string Image { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            this.MouseLeft = new KeyMapping()
            {
                MouseButton = 1,
                OffsetId = -1,
            };
            this.MouseRight = new KeyMapping()
            {
                MouseButton = 0,
                OffsetId = -1,
            };
            this.MouseHorizontal = new KeyMapping() { IsAxisMap = true };
            this.MouseVertical = new KeyMapping() { IsAxisMap = true };
            this.ProfileName = "My Profile";
        }
    }
}
