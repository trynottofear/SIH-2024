# import mediapipe as mp
# import cv2
# import numpy as np
# mp_drawing = mp.solutions.drawing_utils
# mp_pose = mp.solutions.pose
#
#
#
# def calculate_angle(a,b,c):
#     a = np.array(a)
#     b = np.array(b)
#     c = np.array(c)
#
#     radians = np.arctan2(c[1]-b[1],c[0]-b[0]) - np.arctan2(a[1]-b[1],a[0]-b[0])
#     angle = np.abs(radians*180/np.pi)
#
#     if angle>180.0:
#         angle = 360-angle
#
#     return angle
#
#
#
#
#
# cap = cv2.VideoCapture(0)
#
# #Curl counter variables
# counter = 0
# stage = None
#
# #Setup mediapipe instance
# with mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5) as pose:
#     while cap.isOpened():
#         ret, frame = cap.read()
#
#         #Recolor image
#         image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
#         image.flags.writeable = False
#
#         #Make detection
#         results = pose.process(image)
#
#         #Recolor back to BGR
#         image.flags.writeable = True
#         image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)
#
#         #Extract landmarks
#         try:
#             landmarks = results.pose_landmarks.landmark
#             shoulder = [landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].x,
#                         landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].y]
#             elbow = [landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].x,
#                      landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].y]
#             wrist = [landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].x,
#                      landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].y]
#             hip = [landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].x,
#                      landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].y]
#
#             angle= calculate_angle(shoulder, elbow, wrist)
#             angle2=  calculate_angle(elbow, shoulder, hip)
#             #print (angle2)
#
#             #Visualise angle
#             cv2.putText(image, str(angle), tuple(np.multiply(elbow,[640,480]).astype(int)), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255,255,255),2,cv2.LINE_AA)
#
#             #Curl counter
#             if angle2<30 and angle > 160:
#                 stage = 'down'
#             if angle2<30 and angle < 40 and stage=='down':
#                 stage='up'
#                 counter+=1
#                 print(counter)
#
#
#         except:
#             pass
#
#         #Render curl counter
#         #Setup status box
#         cv2.rectangle(image,(0,0), (225,73), (245,117,16), -1)
#
#
#         #Rep data
#         cv2.putText(image, 'REPS', (15,12), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0,0,0), 1, cv2.LINE_AA)
#         cv2.putText(image, str(counter), (10, 60), cv2.FONT_HERSHEY_SIMPLEX, 1.5, (255, 255, 255), 2, cv2.LINE_AA)
#
#         # Stage data
#         cv2.putText(image, 'STAGE', (100, 12), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 0), 1, cv2.LINE_AA)
#         cv2.putText(image, stage, (90, 60), cv2.FONT_HERSHEY_SIMPLEX, 1.5, (255, 255, 255), 2, cv2.LINE_AA)
#
#         #Render Detections
#         mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS,
#                                   mp_drawing.DrawingSpec(color=(245,117,66), thickness=2, circle_radius=2),
#                                   mp_drawing.DrawingSpec(color=(245,66,230), thickness=2, circle_radius=2))
#
#         cv2.imshow('Mediapipe Feed', image)
#
#         if cv2.waitKey(10) & 0xFF == ord('q'):
#             break
#
#     cap.release()
#     cv2.destroyAllWindows()


import mediapipe as mp
import cv2
import numpy as np
mp_drawing = mp.solutions.drawing_utils
mp_pose = mp.solutions.pose



def calculate_angle(a,b,c):
    a = np.array(a)
    b = np.array(b)
    c = np.array(c)

    radians = np.arctan2(c[1]-b[1],c[0]-b[0]) - np.arctan2(a[1]-b[1],a[0]-b[0])
    angle = np.abs(radians*180/np.pi)

    if angle>180.0:
        angle = 360-angle

    return angle





cap = cv2.VideoCapture(0)

# Get the default width and height of the video feed
# frame_width = int(cap.get(3))  # 3 corresponds to CAP_PROP_FRAME_WIDTH
# frame_height = int(cap.get(4))  # 4 corresponds to CAP_PROP_FRAME_HEIGHT
frame_width = 1280  # Increased width
frame_height = 720  # Increased height

# Define the codec and create a VideoWriter object
# Use 'mp4v' codec for saving the file as an MP4 file
fourcc = cv2.VideoWriter_fourcc(*'mp4v')
out = cv2.VideoWriter('output1.mp4', fourcc, 20.0, (frame_width, frame_height))




#Curl counter variables
counter = 0
stage = None

#Setup mediapipe instance
with mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5) as pose:
    while cap.isOpened():
        ret, frame = cap.read()

        #Recolor image
        image = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        image.flags.writeable = False

        #Make detection
        results = pose.process(image)

        #Recolor back to BGR
        image.flags.writeable = True
        image = cv2.cvtColor(image, cv2.COLOR_RGB2BGR)

        #Extract landmarks
        try:
            landmarks = results.pose_landmarks.landmark
            shoulder = [landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].x,
                        landmarks[mp_pose.PoseLandmark.LEFT_SHOULDER.value].y]
            elbow = [landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].x,
                     landmarks[mp_pose.PoseLandmark.LEFT_ELBOW.value].y]
            wrist = [landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].x,
                     landmarks[mp_pose.PoseLandmark.LEFT_WRIST.value].y]
            hip = [landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].x,
                     landmarks[mp_pose.PoseLandmark.LEFT_HIP.value].y]

            angle= calculate_angle(shoulder, elbow, wrist)
            angle2=  calculate_angle(elbow, shoulder, hip)
            #print (angle2)

            #Visualise angle
            cv2.putText(image, str(angle), tuple(np.multiply(elbow,[640,480]).astype(int)), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255,255,255),2,cv2.LINE_AA)

            #Curl counter
            if angle2<30 and angle > 160:
                stage = 'down'
            if angle2<30 and angle < 40 and stage=='down':
                stage='up'
                counter+=1
                print(counter)


        except:
            pass

        #Render curl counter
        #Setup status box
        cv2.rectangle(image,(0,0), (225,73), (245,117,16), -1)


        #Rep data
        cv2.putText(image, 'REPS', (15,12), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0,0,0), 1, cv2.LINE_AA)
        cv2.putText(image, str(counter), (10, 60), cv2.FONT_HERSHEY_SIMPLEX, 1.5, (255, 255, 255), 2, cv2.LINE_AA)

        # Stage data
        cv2.putText(image, 'STAGE', (100, 12), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 0), 1, cv2.LINE_AA)
        cv2.putText(image, stage, (90, 60), cv2.FONT_HERSHEY_SIMPLEX, 1.5, (255, 255, 255), 2, cv2.LINE_AA)

        #Render Detections
        mp_drawing.draw_landmarks(image, results.pose_landmarks, mp_pose.POSE_CONNECTIONS,
                                  mp_drawing.DrawingSpec(color=(245,117,66), thickness=2, circle_radius=2),
                                  mp_drawing.DrawingSpec(color=(245,66,230), thickness=2, circle_radius=2))

        # Write the frame to the output video file
        out.write(image)


        cv2.imshow('Mediapipe Feed', image)

        if cv2.waitKey(10) & 0xFF == ord('q'):
            break

    cap.release()
    out.release()
    cv2.destroyAllWindows()
