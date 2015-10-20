/*
 * Copyright (C) 2012 ZXing authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package com.google.zxing.client.android.camera.open;

import com.gft.appverse.android.scanner.CameraPreferences;
import com.google.zxing.client.android.CaptureActivity;

import android.annotation.TargetApi;
import android.app.Activity;
import android.hardware.Camera;
import android.util.Log;
import android.view.Surface;

/**
 * Implementation for Android API 9 (Gingerbread) and later. This opens up the possibility of accessing
 * front cameras, and rotated cameras.
 */
@TargetApi(9)
public final class GingerbreadOpenCameraInterface implements OpenCameraInterface {

  private static final String TAG = "GingerbreadOpenCamera";

  /**
   * Opens a rear-facing camera with {@link Camera#open(int)}, if one exists, or opens camera 0.
   */
  @Override
  public Camera open() {
    
    int numCameras = Camera.getNumberOfCameras();
    if (numCameras == 0) {
      Log.w(TAG, "No cameras!");
      return null;
    }else{
    	Log.w(TAG, "numCameras: "+numCameras);
    }

    boolean front = CameraPreferences.getInstance().isFront();
    int cameraToUse = Camera.CameraInfo.CAMERA_FACING_BACK,
    		orientationToUse = 0;
    
    if(front){
    	cameraToUse = Camera.CameraInfo.CAMERA_FACING_FRONT;
    	orientationToUse = 180;
    }
    
    int index = 0;
    while (index < numCameras) {
    	Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
    	
    	Camera.getCameraInfo(index, cameraInfo);
    	
    	Log.i(TAG,cameraInfo.orientation+" ori");
    	Log.i(TAG,"index: "+index);
    	
    	if (cameraInfo.facing == cameraToUse)
    	{ 
    		break; 
		}
    	/*/Original code (gets the back camera. This is NOT what you want!)
    	if (cameraInfo.facing == Camera.CameraInfo.CAMERA_FACING_BACK)
    	{ break; }
    	*/
    	index++;
    	}
    
    Camera camera;
    if (index < numCameras) {
      Log.i(TAG, "Opening camera #" + index);
      camera = Camera.open(index);
      //camera.setDisplayOrientation(orientationToUse);
    } else {
      Log.i(TAG, "camera back; returning camera #0");
      camera = Camera.open(0);
      index = 0;
    }
    setCameraDisplayOrientation(CaptureActivity.getActivity(), index, camera);
    return camera;
  }
  
  public static void setCameraDisplayOrientation(Activity activity,
	         int cameraId, android.hardware.Camera camera) {
	  
	     try {
			android.hardware.Camera.CameraInfo info =
			         new android.hardware.Camera.CameraInfo();
			 android.hardware.Camera.getCameraInfo(cameraId, info);
			 int rotation = activity.getWindowManager().getDefaultDisplay()
			         .getRotation();
			 int degrees = 0;
			 switch (rotation) {
			     case Surface.ROTATION_0: degrees = 0; break;
			     case Surface.ROTATION_90: degrees = 90; break;
			     case Surface.ROTATION_180: degrees = 180; break;
			     case Surface.ROTATION_270: degrees = 270; break;
			 }

			 int result;
			 if (info.facing == Camera.CameraInfo.CAMERA_FACING_FRONT) {
			     result = (info.orientation + degrees) % 360;
			     result = (360 - result) % 360;  // compensate the mirror
			 } else {  // back-facing
			     result = (info.orientation - degrees + 360) % 360;
			 }
			 camera.setDisplayOrientation(result);
		} catch (Exception e) {
			// TODO Auto-generated catch block
			Log.e(TAG, "Cannot orientate. ex: "+ e.getLocalizedMessage());
		}
	 }

}
