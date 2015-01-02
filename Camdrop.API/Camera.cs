using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camdrop.API
{
    public class Camera
    {
        public string talkback_stream_host { get; set; }
        public bool is_streaming_enabled { get; set; }
        public string timezone { get; set; }
        public int id { get; set; }
        public string live_stream_host { get; set; }
        public string description { get; set; }
        public string uuid { get; set; }
        public string title { get; set; }
        public int trial_days_left { get; set; }
        public List<string> capabilities { get; set; }
        public bool is_trial_mode { get; set; }
        public string location { get; set; }
        public string mac_address { get; set; }
        public bool is_trial_warning { get; set; }
        public int type { get; set; }
        public bool has_bundle { get; set; }
        public string owner_id { get; set; }
        public string last_local_ip { get; set; }
        public bool is_streaming { get; set; }
        public double timezone_utc_offset { get; set; }
        public bool is_online { get; set; }
        public bool is_public { get; set; }
        public string download_host { get; set; }
        public string name { get; set; }
        public bool is_connected { get; set; }
        public string nexus_api_http_server { get; set; }
        public double hours_of_recording_max { get; set; }
        public string where { get; set; }
    }
}
