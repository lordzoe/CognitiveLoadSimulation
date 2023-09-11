    using UnityEngine;
    using System.IO;
    using ViveSR.anipal.Eye;

    public class EyeTracking : MonoBehaviour
    {
        private bool isEyeTrackingAvailable = false;
        private string csvfileName;
        private StreamWriter csvwriter;

        void Start()
        {
            // Initialize the SRanipal eye framework
            SRanipal_Eye_Framework.Instance.StartFramework();

            // Check if eye tracking is available with SRanipal
            isEyeTrackingAvailable = SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING;

            // Create a new CSV file
            if (isEyeTrackingAvailable)
            {
                csvfileName = $"EyeTrackingData_{System.DateTime.Now:yyyy_MM_dd_HH_mm_ss}.csv";
                string filePath = Path.Combine(Application.dataPath, "CSVOutput", csvfileName);

                // Create the CSV file and write the header
                csvwriter = new StreamWriter(filePath, append: false);
                csvwriter.WriteLine("Timestamp, FixationPointX, FixationPointY, FixationPointZ," +
                                  "LeftGazeX, LeftGazeY, LeftGazeZ, RightGazeX, RightGazeY, RightGazeZ," +
                                  "LeftGazeOriginX, LeftGazeOriginY, LeftGazeOriginZ, RightGazeOriginX, RightGazeOriginY, RightGazeOriginZ," +
                                  "LeftPupilDiameter, RightPupilDiameter, LeftEyeOpenness, RightEyeOpenness");
                csvwriter.Flush();
            }
        }

        void Update()

        {
            
                if (SRanipal_Eye_Framework.Status == SRanipal_Eye_Framework.FrameworkStatus.WORKING)
                {
                    // Extract and write eye tracking data to CSV file
                }
            
            if (isEyeTrackingAvailable)
            {
                EyeData eyeData = new EyeData();
                if (SRanipal_Eye_API.GetEyeData(ref eyeData) == ViveSR.Error.WORK)
                {
                    // Extract eye tracking information
                    var timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                    Vector3 leftGaze = eyeData.verbose_data.left.gaze_direction_normalized;
                    Vector3 rightGaze = eyeData.verbose_data.right.gaze_direction_normalized;
                    Vector3 leftGazeOrigin = eyeData.verbose_data.left.gaze_origin_mm;
                    Vector3 rightGazeOrigin = eyeData.verbose_data.right.gaze_origin_mm;
                    Vector3 fixationPoint = (leftGaze + rightGaze) / 2;
                    float leftPupilDiameter = eyeData.verbose_data.left.pupil_diameter_mm;
                    float rightPupilDiameter = eyeData.verbose_data.right.pupil_diameter_mm;
                    float leftEyeOpenness = eyeData.verbose_data.left.eye_openness;
                    float rightEyeOpenness = eyeData.verbose_data.right.eye_openness;

                    // Write the extracted data to the CSV file
                    csvwriter.WriteLine($"{timestamp}, {fixationPoint.x}, {fixationPoint.y}, {fixationPoint.z}," +
                                      $"{leftGaze.x}, {leftGaze.y}, {leftGaze.z}, {rightGaze.x}, {rightGaze.y}, {rightGaze.z}," +
                                      $"{leftGazeOrigin.x}, {leftGazeOrigin.y}, {leftGazeOrigin.z}, {rightGazeOrigin.x}, {rightGazeOrigin.y}, {rightGazeOrigin.z}," +
                                      $"{leftPupilDiameter}, {rightPupilDiameter}, {leftEyeOpenness}, {rightEyeOpenness}");
                    csvwriter.Flush();
                }
            }
        }

        void OnApplicationQuit()
        {
            // Stop the SRanipal eye framework
            if (isEyeTrackingAvailable)
            {
                SRanipal_Eye_Framework.Instance.StopFramework();
                csvwriter.Close();
            }
        }
    }