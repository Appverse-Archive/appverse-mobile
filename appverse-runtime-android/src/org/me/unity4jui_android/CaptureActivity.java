/*
 Copyright (c) 2012 GFT Appverse, S.L., Sociedad Unipersonal.

 This Source  Code Form  is subject to the  terms of  the Appverse Public License 
 Version 2.0  ("APL v2.0").  If a copy of  the APL  was not  distributed with this 
 file, You can obtain one at http://appverse.org/legal/appverse-license/.

 Redistribution and use in  source and binary forms, with or without modification, 
 are permitted provided that the  conditions  of the  AppVerse Public License v2.0 
 are met.

 THIS SOFTWARE IS PROVIDED BY THE  COPYRIGHT HOLDERS  AND CONTRIBUTORS "AS IS" AND
 ANY EXPRESS  OR IMPLIED WARRANTIES, INCLUDING, BUT  NOT LIMITED TO,   THE IMPLIED
 WARRANTIES   OF  MERCHANTABILITY   AND   FITNESS   FOR A PARTICULAR  PURPOSE  ARE
 DISCLAIMED. EXCEPT IN CASE OF WILLFUL MISCONDUCT OR GROSS NEGLIGENCE, IN NO EVENT
 SHALL THE  COPYRIGHT OWNER  OR  CONTRIBUTORS  BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL,  SPECIAL,   EXEMPLARY,  OR CONSEQUENTIAL DAMAGES  (INCLUDING, BUT NOT
 LIMITED TO,  PROCUREMENT OF SUBSTITUTE  GOODS OR SERVICES;  LOSS OF USE, DATA, OR
 PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
 WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) 
 ARISING  IN  ANY WAY OUT  OF THE USE  OF THIS  SOFTWARE,  EVEN  IF ADVISED OF THE 
 POSSIBILITY OF SUCH DAMAGE.
 */
package org.me.unity4jui_android;

import java.io.ByteArrayOutputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.lang.reflect.Method;

import com.gft.unity.android.AndroidCameraOptions;
import com.gft.unity.android.AndroidMedia;
import com.gft.unity.android.AndroidSystemLogger;
import com.gft.unity.core.system.SystemLogger.Module;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Color;
import android.graphics.Matrix;
import android.graphics.PorterDuff.Mode;
import android.graphics.Typeface;
import android.graphics.drawable.Drawable;
import android.hardware.Camera;
import android.hardware.Camera.PictureCallback;
import android.media.ExifInterface;
import android.net.Uri;
import android.os.Bundle;
import android.provider.MediaStore;
import android.util.Log;
import android.view.Gravity;
import android.view.MotionEvent;
import android.view.Surface;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnTouchListener;
import android.view.Window;
import android.widget.EditText;
import android.widget.FrameLayout;
import android.widget.FrameLayout.LayoutParams;
import android.widget.ImageButton;
import android.widget.TextView;

public class CaptureActivity extends Activity implements SurfaceHolder.Callback {

	private static final AndroidSystemLogger LOG = AndroidSystemLogger.getSuperClassInstance();

	private SurfaceView mSurfaceView = null;
	private SurfaceHolder mHolder = null;
	private Camera camera = null;
	private ImageButton btnTakePicture = null;
	private ImageButton btnScanBG = null;
	private ImageButton btnScan = null;

	private Boolean overlay = false;
	private Boolean front = false;
	private Uri picturePath = null;
	private int tint = 0;

	private AndroidCameraOptions cameraOptions;

	private TextView cancelTextButton;
	private EditText helpText;
	private boolean mSurfaceExists;
	float scale;

	private int selectedCamera;

	/**
     * Initialize the camera preview
     */
    private void initPreview() {

        if (camera != null) {

            try {
            	camera.setPreviewDisplay(mHolder);
            } catch (IOException e) {
                // log
            }

            camera.startPreview();
        }
    }
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		LOG.LogDebug(Module.GUI, "onCreate");

		scale = getResources().getDisplayMetrics().density;
		getWindow().requestFeature(Window.FEATURE_NO_TITLE); // do not show top
																// title bar

		int layoutId = getResources().getIdentifier("takesnapshot", "layout", getPackageName());
		setContentView(layoutId);

		int surfaceViewId = getResources().getIdentifier("preview_view", "id", getPackageName());

		int buttonId = getResources().getIdentifier("btnTakePicture", "id", getPackageName());
		btnTakePicture = (ImageButton) findViewById(buttonId);

		int btnScanBGId = getResources().getIdentifier("btnScanBG", "id", getPackageName());
		btnScanBG = (ImageButton) findViewById(btnScanBGId);

		int btnScanId = getResources().getIdentifier("btnScan", "id", getPackageName());
		btnScan = (ImageButton) findViewById(btnScanId);

		OnClickListener shot = new OnClickListener() {

			@Override
			public void onClick(View v) {
				LOG.LogDebug(Module.GUI, "shot onClick");
				if (camera != null) {
					LOG.LogDebug(Module.GUI, "btnTakePicture onClick - taking picture...");
					camera.takePicture(null, null, new AppversePictureCallback());
				} else {
					LOG.LogDebug(Module.GUI, "btnTakePicture onClick - no camera available");
					finish();
				}
			}
		};

		OnTouchListener tap = new OnTouchListener() {

			@Override
			public boolean onTouch(View v, MotionEvent event) {
				switch (event.getAction()) {
				case MotionEvent.ACTION_DOWN: {
					Log.d("CaptureActivity", "ACTION_DOWN");
					if (cameraOptions != null) {
						tint = Color.parseColor(cameraOptions.getScanButtonPressedColorHexadecimal());
						btnScanBG.setColorFilter(tint, Mode.MULTIPLY);
					}
					camera.takePicture(null, null, new AppversePictureCallback());
					break;
				}
				case MotionEvent.ACTION_CANCEL: {
					// DO NOTHING
					Log.d("CaptureActivity", "ACTION_CANCEL");
					break;
				}
				}
				return true;
			}
		};
		btnScanBG.setOnTouchListener(tap);
		btnScan.setOnTouchListener(tap);

		btnTakePicture.setOnClickListener(shot);
		// btnScanBG.setOnClickListener(shot);
		// btnScan.setOnClickListener(shot);

		int cancelTextButtonId = getResources().getIdentifier("cancelTextButton", "id", getPackageName());
		cancelTextButton = (TextView) findViewById(cancelTextButtonId);
		cancelTextButton.setOnClickListener(new View.OnClickListener() {

			@Override
			public void onClick(View v) {
				LOG.LogDebug(Module.GUI, "Cancel onClick");
				// onDestroy(); THIS DO NOTHING
				releaseCamera();
				finish();
			}

		});

		mSurfaceView = (SurfaceView) findViewById(surfaceViewId);

		
	}
	
	

	@Override
	public void onWindowAttributesChanged(android.view.WindowManager.LayoutParams params) {
		mSurfaceExists = false;
		super.onWindowAttributesChanged(params);
	}



	@Override
	protected void onResume() {
		super.onResume();

		LOG.LogDebug(Module.GUI, "onResume");

		// Drawable d = Drawable.createFromPath("@drawable/cameraicon");
		// btnTakePicture.setImageDrawable(d);
		if(!mSurfaceExists){
			LOG.LogDebug(Module.GUI, "DONT mSurfaceExists");
			Intent data = getIntent();
			if (data != null) {
				picturePath = (Uri) data.getParcelableExtra(MediaStore.EXTRA_OUTPUT);
				if (picturePath != null)
					LOG.LogDebug(Module.GUI, "Using picture path: " + picturePath.toString());
				else
					LOG.LogDebug(Module.GUI, "No picture path received");
	
				cameraOptions = data.getParcelableExtra(AndroidMedia.OVERLAY);
				LOG.LogDebug(Module.GUI, "AndroidCameraOptions: " + cameraOptions.toString());
	
				front = cameraOptions.getUseFrontCamera();
				LOG.LogDebug(Module.GUI, "Use Front Camera: " + front);
	
				// overlay = (Boolean) data.getBooleanExtra(AndroidMedia.OVERLAY,
				// false);
				overlay = cameraOptions.getUseCustomCameraOverlay();
				LOG.LogDebug(Module.GUI, "Use Overlay: " + overlay);
	
				int scanLayoutId = getResources().getIdentifier("scanLayout", "id", getPackageName());
				FrameLayout scanLayout = (FrameLayout) findViewById(scanLayoutId);
	
				int scanOverlayLayoutId = getResources().getIdentifier("scanOverlayLayout", "id", getPackageName());
				FrameLayout scanOverlayLayout = (FrameLayout) findViewById(scanOverlayLayoutId);
	
				int scanButtonLayoutId = getResources().getIdentifier("scan_button_holder", "id", getPackageName());
				FrameLayout scanButtonLayout = (FrameLayout) findViewById(scanButtonLayoutId);
	
				int photoLayoutId = getResources().getIdentifier("photo_button_holder", "id", getPackageName());
				FrameLayout photoButtonLayout = (FrameLayout) findViewById(photoLayoutId);
	
				int helpTextId = getResources().getIdentifier("helpText", "id", getPackageName());
				helpText = (EditText) findViewById(helpTextId);
	
				if (overlay) {
					try {
	
						/*
						 * /ButtonBG size LOG.LogDebug(Module.GUI, "ButtonBG size");
						 * btnScanBG.getLayoutParams().width =
						 * px2dp(cameraOptions.getScanButtonWidth());
						 * btnScanBG.getLayoutParams().height =
						 * px2dp(cameraOptions.getScanButtonHeight());
						 * btnScanBG.requestLayout();
						 */
	
						/*
						 * /Bottom margin LOG.LogDebug(Module.GUI,
						 * "Button Bottom margin l: "+lp.leftMargin+" t: "
						 * +lp.topMargin+" r: "+lp.rightMargin+" b: "
						 * +lp.bottomMargin); FrameLayout.LayoutParams lp = new
						 * FrameLayout.LayoutParams(FrameLayout.LayoutParams.
						 * WRAP_CONTENT, FrameLayout.LayoutParams.WRAP_CONTENT);
						 * lp.bottomMargin =
						 * cameraOptions.getScanButtonMarginBottom();
						 * btnScanBG.setLayoutParams(lp); btnScanBG.requestLayout();
						 */
	
						/*
						 * /IconButton size LOG.LogDebug(Module.GUI,
						 * "IconButton size"); btnScan.getLayoutParams().width =
						 * px2dp(cameraOptions.getScanButtonIconWidth());
						 * btnScan.getLayoutParams().height =
						 * px2dp(cameraOptions.getScanButtonIconHeight());
						 * btnScan.requestLayout();
						 */
						
	
						// btnBG change Color
						LOG.LogDebug(Module.GUI, "btnBG change Color");
						tint = Color.parseColor(cameraOptions.getScanButtonColorHexadecimal());
						btnScanBG.setColorFilter(tint);
	
						// btnIcon change color
						LOG.LogDebug(Module.GUI, "btnIcon change color");
						tint = Color.parseColor(cameraOptions.getScanButtonIconColorHexadecimal());
						btnScan.setColorFilter(tint);
	
						// Overlay MARGINS
						int margin = cameraOptions.getGuidelinesMargins();
						LOG.LogDebug(Module.GUI, "Overlay margin: " + margin);
						LayoutParams params = (LayoutParams) scanOverlayLayout.getLayoutParams();
						params.setMargins(margin, margin, margin, margin);
						scanOverlayLayout.setLayoutParams(params);
						scanOverlayLayout.requestLayout();
	
						// Overlay Color
						LOG.LogDebug(Module.GUI, "Overlay color");
						tint = Color.parseColor(cameraOptions.getGuidelinesColorHexadecimal());
						String overlayImage = cameraOptions.getOverlay();
						int backgroundId = 0;
						// FIX 23/11/2015 (maps) - When the overlay is not set in the JSON, this arrive here as the string "null", not as a null value.
						// I think that the parceable data echange between the Media API and the CaptureActivity is the culprit
						LOG.LogDebug(Module.GUI, "Checking overlay to use...");
						if(overlayImage == null || overlayImage.toLowerCase().equals("null")) {
							LOG.LogDebug(Module.GUI, "No custom overlay provided. Using default one (background)");
							overlayImage = "background";
						}
						LOG.LogDebug(Module.GUI, "Overlay: "+overlayImage);
							
						try
						{
							backgroundId = getResources().getIdentifier(overlayImage, "drawable", getPackageName());
						}catch(Exception ex){
							LOG.LogDebug(Module.GUI, "Overlay: " + overlay +" Not found: "+ex.getMessage());
						}
						
						Drawable background = getResources().getDrawable(backgroundId);
						background.mutate().setColorFilter(tint, Mode.MULTIPLY);
						scanOverlayLayout.setBackground(background);
	
						// helpText margins
						LOG.LogDebug(Module.GUI, "helpText margins");
						params = (LayoutParams) helpText.getLayoutParams();
						params.gravity = Gravity.BOTTOM;
						params.leftMargin = cameraOptions.getDescriptionLabelMarginLeftRight();
						params.rightMargin = cameraOptions.getDescriptionLabelMarginLeftRight();
						params.bottomMargin = cameraOptions.getDescriptionLabelMarginBottom();
						helpText.setLayoutParams(params);
						helpText.requestLayout();
	
						// helpText Height
						LOG.LogDebug(Module.GUI, "helpText Height");
						helpText.setHeight(px2dp(cameraOptions.getDescriptionLabelHeight()));
						helpText.requestLayout();
	
						// helpText Text & Color
						LOG.LogDebug(Module.GUI, "helpText Text & Color");
						helpText.setText(cameraOptions.getDescriptionLabelText());
						helpText.setTextColor(Color.parseColor(cameraOptions.getDescriptionLabelColorHexadecimal()));
	
						// helpText Font
						LOG.LogDebug(Module.GUI, "helpText Font");
						if (!cameraOptions.getDescriptionLabelFontFamilyName().equalsIgnoreCase("Helvetica")) {
							try {
								String font = cameraOptions.getDescriptionLabelFontFamilyName();
								Typeface myFont = Typeface.createFromAsset(getAssets(), "fonts/" + font + ".ttf");
								helpText.setTypeface(myFont);
							} catch (Exception e) {
								LOG.Log("Font not found for description", e);
							}
						}
	
						// helpText Font size
						LOG.LogDebug(Module.GUI, "helpText Font size");
						helpText.setTextSize(cameraOptions.getDescriptionLabelFontSize());
	
						// cancelTextButton Text & Color
						LOG.LogDebug(Module.GUI, "cancelTextButton Text & Color");
						cancelTextButton.setText(cameraOptions.getCancelButtonText());
						cancelTextButton.setTextColor(Color.parseColor(cameraOptions.getCancelButtonColorHexadecimal()));
	
						// cancelTextButton size
						LOG.LogDebug(Module.GUI, "cancelTextButton size");
						cancelTextButton.setWidth(px2dp(cameraOptions.getCancelButtonWidth()));
						cancelTextButton.setHeight(px2dp(cameraOptions.getCancelButtonHeight()));
						cancelTextButton.requestLayout();
	
						// cancelTextButton Font
						LOG.LogDebug(Module.GUI, "cancelTextButton Font");
						if (!cameraOptions.getCancelButtonFontFamilyName().equalsIgnoreCase("Helvetica")) {
							try {
								String font = cameraOptions.getCancelButtonFontFamilyName();
								Typeface myFont = Typeface.createFromAsset(getAssets(), "fonts/" + font + ".ttf");
								cancelTextButton.setTypeface(myFont);
							} catch (Exception e) {
								LOG.Log("Font not found for cancel", e);
							}
						}
	
						// cancelTextButton Font size
						LOG.LogDebug(Module.GUI, "cancelTextButton Font size");
						cancelTextButton.setTextSize(cameraOptions.getCancelButtonFontSize());
	
					} catch (Exception e) {
						LOG.Log("Error applying overlay changes", e);
					}
	
					scanLayout.setVisibility(View.VISIBLE);
					scanButtonLayout.setVisibility(View.VISIBLE);
					scanOverlayLayout.setVisibility(View.VISIBLE);
	
					photoButtonLayout.setVisibility(View.INVISIBLE);
	
				} else {
	
					scanLayout.setVisibility(View.INVISIBLE);
					scanButtonLayout.setVisibility(View.INVISIBLE);
					scanOverlayLayout.setVisibility(View.INVISIBLE);
	
					photoButtonLayout.setVisibility(View.VISIBLE);
	
				}
	
			}
	
			// NOT WORKING FOR ALL ANDROID DEVICES, cannot be used
			// intent.putExtra("android.intent.extras.CAMERA_FACING",
			// android.hardware.Camera.CameraInfo.CAMERA_FACING_FRONT);
	
			int numCameras = Camera.getNumberOfCameras();
			if (numCameras == 0) {
				LOG.LogDebug(Module.GUI, "No cameras found on this device!");
				return;
			} else {
				LOG.LogDebug(Module.GUI, "numCameras: " + numCameras);
			}
	
			int index = 0;
			while (index < numCameras) {
				Camera.CameraInfo cameraInfo = new Camera.CameraInfo();
	
				Camera.getCameraInfo(index, cameraInfo);
	
				// LOG.LogDebug(Module.GUI, cameraInfo.orientation+" ori");
				// LOG.LogDebug(Module.GUI, "index: "+index);
				if (front) {
					if (cameraInfo.facing == Camera.CameraInfo.CAMERA_FACING_FRONT) {
						break;
					}
				} else {
					if (cameraInfo.facing == Camera.CameraInfo.CAMERA_FACING_BACK) {
						break;
					}
				}
				index++;
			}
			
			if (index < numCameras) {
				LOG.LogDebug(Module.GUI, "Opening camera #" + index);// +"
				selectedCamera = index;										// orientation:
																		// "+result
				camera = Camera.open(selectedCamera);
				setCameraDisplayOrientation(this, index, camera);
				/*
				 * Camera.Parameters parameters = camera.getParameters();
				 * parameters.set("orientation", "portrait");
				 * camera.setParameters(parameters);
				 */
				/*
				 * if(front){ List<String> devicesFix = new ArrayList<String>();
				 * devicesFix.add("Nexus 6"); //LOG.LogDebug(Module.GUI, "Device: "
				 * + Build.MODEL);//+" orientation: "+result int result;
				 * if(devicesFix.contains(Build.MODEL)){ result = 270; }else result
				 * = 90; //camera.setDisplayOrientation(result); //270 KO - 0 KO -
				 * 180 KO
				 * 
				 * }else{ //camera.setDisplayOrientation(90); }
				 */
	
			} else {
				LOG.LogDebug(Module.GUI, "Opening camera #0");
				selectedCamera = 0;
				camera = Camera.open(selectedCamera);
			}
			
			try {
				Camera.Parameters params = camera.getParameters();
				//*EDIT*//params.setFocusMode("continuous-picture");
				//It is better to use defined constraints as opposed to String, thanks to AbdelHady
				params.setFocusMode(Camera.Parameters.FOCUS_MODE_CONTINUOUS_PICTURE);
				if(camera != null)
					camera.setParameters(params);
			} catch (Exception e) {
				LOG.LogDebug("captureActiviy - Set Parameters failure: "+e.getMessage());
			}
			
			// Install a SurfaceHolder.Callback so we get notified when the
			// underlying surface is created and destroyed.
			mHolder = mSurfaceView.getHolder();
			mHolder.addCallback(this);
			mHolder.setType(SurfaceHolder.SURFACE_TYPE_PUSH_BUFFERS);
			
			
		}
		initPreview();
		

	}

	protected static void setDisplayOrientation(Camera camera, int angle) {
		Method downPolymorphic;
		try {
			downPolymorphic = camera.getClass().getMethod("setDisplayOrientation", new Class[] { int.class });
			if (downPolymorphic != null)
				downPolymorphic.invoke(camera, new Object[] { angle });
		} catch (Exception e1) {
		}
	}

	private int px2dp(int size) {
		int dp = (int) (size * scale + 0.5f);
		LOG.LogDebug(Module.GUI, "px2dp px: " + size + " -> dp: " + dp);
		return dp;
	}

	@Override
	public void surfaceChanged(SurfaceHolder holder, int format, int w, int h) {

		LOG.LogDebug(Module.GUI, "surfaceChanged  w: " + w + ", h:" + h);

		// Now that the size is known, set up the camera parameters and begin
		// the preview.
		/*
		 * Camera.Parameters parameters = camera.getParameters();
		 * parameters.setPreviewSize(w, h); //requestLayout();
		 * camera.setParameters(parameters);
		 */

		// Important: Call startPreview() to start updating the preview surface.
		// Preview must be started before you can take a picture.
		//camera.startPreview();
		initPreview();
	}

	@Override
	public void surfaceCreated(SurfaceHolder arg0) {
		LOG.LogDebug(Module.GUI, "surfaceCreated");
		mSurfaceExists = true;
		try {
			camera.setPreviewDisplay(mHolder);

		} catch (IOException e) {
			LOG.Log("surfaceCreated", e);
		}
	}

	@Override
	public void surfaceDestroyed(SurfaceHolder arg0) {
		LOG.LogDebug(Module.GUI, "surfaceDestroyed");
		releaseCamera();
		mSurfaceExists = false;
	}

	private void releaseCamera() {
		LOG.LogDebug(Module.GUI, "releaseCamera");
		if (camera != null) {
			camera.stopPreview();
			camera.release();
			camera = null;
		}
	}
	
	public static void setCameraDisplayOrientation(Activity activity, int cameraId, android.hardware.Camera camera) {
		android.hardware.Camera.CameraInfo info = new android.hardware.Camera.CameraInfo();
		// Log.d("Camera", "orientation: "+info.orientation);
		android.hardware.Camera.getCameraInfo(cameraId, info);
		LOG.LogDebug(Module.GUI, "[Camera] info.orientation: "+info.orientation);
		
		
		int rotation = activity.getWindowManager().getDefaultDisplay().getRotation();
		// Log.d("Camera", "rotation: "+rotation);
		int degrees = 0;
		switch (rotation) {
		case Surface.ROTATION_0:
			degrees = 0;
			break;
		case Surface.ROTATION_90:
			degrees = 90;
			break;
		case Surface.ROTATION_180:
			degrees = 180;
			break;
		case Surface.ROTATION_270:
			degrees = 270;
			break;
		}

		int result;
		if (info.facing == Camera.CameraInfo.CAMERA_FACING_FRONT) {
			result = (info.orientation + degrees) % 360;
			result = (360 - result) % 360; // compensate the mirror
		} else { // back-facing
			result = (info.orientation - degrees + 360) % 360;
		}
		LOG.LogDebug(Module.GUI, "[Camera] setDisplayOrientation: "+result);
		camera.setDisplayOrientation(result);
	}

	private class AppversePictureCallback implements PictureCallback {

		@Override
		public void onPictureTaken(byte[] pictureBuffer, Camera camera) {
			if (pictureBuffer != null) {
				LOG.LogDebug(Module.GUI, "AppversePictureCallback # byte arrray length " + pictureBuffer.length);

			} else
				LOG.LogDebug(Module.GUI, "AppversePictureCallback # no byte array received");

			try {

				LOG.LogDebug(Module.GUI, "AppversePictureCallback # BitmatRotate");

				Matrix matrix = new Matrix();

				ExifInterface exif = new ExifInterface(picturePath.getPath());     //Since API Level 5
				String exifOrientation = exif.getAttribute(ExifInterface.TAG_ORIENTATION);
				
				int orientation = exif.getAttributeInt(ExifInterface.TAG_ORIENTATION,
		                ExifInterface.ORIENTATION_NORMAL);
				LOG.LogDebug(Module.GUI,"[Camera] exifOrientation: "+exifOrientation);
				LOG.LogDebug(Module.GUI,"[Camera] orientation: "+orientation);
				
				
				//Best actual solution, does not solve the issue
				if(front)
					matrix.postRotate(270);
				else
					matrix.postRotate(90);
				
				ByteArrayOutputStream stream = new ByteArrayOutputStream();
				Bitmap rotatedBitmap = getSmallerRotatedBitmap(pictureBuffer, matrix, 1);
				
				if(rotatedBitmap!= null) {
					LOG.LogDebug(Module.GUI, "AppversePictureCallback # BitmatCompress");
					rotatedBitmap.compress(Bitmap.CompressFormat.JPEG, 100, stream);
				}
				
				byte[] byteArray = stream.toByteArray();

				LOG.LogDebug(Module.GUI, "AppversePictureCallback # BitmatSave");
				FileOutputStream outStream = new FileOutputStream(picturePath.getPath());
				outStream.write(byteArray);// pictureBuffer

				outStream.flush();
				outStream.close();
				outStream = null;
			} catch (FileNotFoundException e) {
				LOG.LogDebug(Module.GUI, "AppversePictureCallback # File not found exception: " + e.getMessage());
			} catch (IOException e) {
				LOG.LogDebug(Module.GUI, "AppversePictureCallback # IO exception: " + e.getMessage());
			}

			// releaseCamera(); //already implemented onDestroy
			Intent data = new Intent(); // pass other params if necessary
			setResult(RESULT_OK, data);

			finish();
		}
		
		/**
		 * Get the appropriate size of the bitmap that fits the device memory allocations
		 * @param pictureBuffer
		 * @param matrix
		 * @param inSampleSize
		 * @return
		 */
		private Bitmap getSmallerRotatedBitmap(byte[] pictureBuffer, Matrix matrix, int inSampleSize) {
		
			Bitmap rotatedBitmap = null;
			Bitmap bitmap = null;
			try {
				if(inSampleSize > 1) {
					LOG.LogDebug(Module.GUI, "Requesting the decoder to subsample the sample the original image, returning a smaller image to save memory... inSampleSize: " + inSampleSize);
					BitmapFactory.Options options = new BitmapFactory.Options();
					options.inSampleSize = inSampleSize;
					
					bitmap = BitmapFactory.decodeByteArray(pictureBuffer, 0, pictureBuffer.length, options);
				} else {
					bitmap = BitmapFactory.decodeByteArray(pictureBuffer, 0, pictureBuffer.length);
				}
				
				rotatedBitmap = Bitmap.createBitmap(bitmap, 0, 0, bitmap.getWidth(), bitmap.getHeight(), matrix, true);
				
			}  catch (OutOfMemoryError ofm) {
				LOG.LogDebug(Module.GUI, "AppversePictureCallback # Getting out of memory exception creating bitmap : " + ofm.getMessage());
				
				return this.getSmallerRotatedBitmap(pictureBuffer, matrix, inSampleSize * 2);
				
			}
			return rotatedBitmap;
		}

	}

	@Override
	protected void onPause() {
		
		//camera.stopPreview();
		//releaseCamera();
		super.onPause();
		
		//finish();
	}
		
	

	@Override
	protected void onDestroy() {
		super.onDestroy();
		LOG.LogDebug(Module.GUI, "onDestroy");
		releaseCamera();

	}

	public void onClickCancel(View v) {
		LOG.LogDebug(Module.GUI, "Annulla Clicked");
		// onDestroy();
	}

}
