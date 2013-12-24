package com.gft.androidnativetest;

import android.os.Bundle;
import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.content.pm.PackageManager.NameNotFoundException;
import android.util.Log;
import android.view.View;
import android.view.Menu;
import android.widget.EditText;
import android.widget.TextView;


public class MainActivity extends Activity {
	private final String PACKAGE_NAME = "com.gft.appverse.showcase";
	private final String PREFERENCES_FILE_NAME = "AppverseSettings";

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
	}

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.main, menu);
		return true;
	}
	
	public void createPreferenceEntry(View v){
		SharedPreferences settings = GetOtherAppSharedPreferences();
		TextView resultLabel = (TextView)findViewById(R.id.sharedResults_label);
		EditText keyLabel = (EditText)findViewById(R.id.sharedPreferences_label);

		if(settings!=null){
			String keyname =keyLabel.getText().toString();
			if(keyname!=null && keyname !=""){
				Editor ed = settings.edit();
				ed.putString(keyname, "MY VALUE COMES FROM THE NATIVE APP");
				ed.commit();
				resultLabel.setText("ADDED THE KEY: " + keyname);
			}else{resultLabel.setText("TEXT EMPTY");}
		}else{
			resultLabel.setText("EMPTY SETTINGS");		}
	}
	
	public void readPreferenceEntry(View v){
		try {
			SharedPreferences settings = GetOtherAppSharedPreferences();
			TextView resultLabel = (TextView)findViewById(R.id.sharedResults_label);
			EditText keyLabel = (EditText)findViewById(R.id.sharedPreferences_label);
			if(settings!=null){
				String keyname =keyLabel.getText().toString();
				if(settings.contains(keyname)){
					resultLabel.setText(keyname + " has value " + settings.getString(keyname, "NO FOUND"));
				}else{
					resultLabel.setText("Nothing found");
				}
			}else{resultLabel.setText("EMPTY SETTINGS");}
		} catch (Exception e) {
			// TODO Auto-generated catch block
		}
	}
	
	public void deletePreferenceEntry(View v){
		SharedPreferences settings = GetOtherAppSharedPreferences();
		TextView resultLabel = (TextView)findViewById(R.id.sharedResults_label);
		EditText keyLabel = (EditText)findViewById(R.id.sharedPreferences_label);
		if(settings!=null){
			String keyname =keyLabel.getText().toString();
			if(settings.contains(keyname)){
				Editor ed = settings.edit();
				ed.remove(keyname);
				ed.commit();
				resultLabel.setText("DELETED THE KEY: " + keyname);
			}else{
				resultLabel.setText("NO KEY TO DELETE");
				}
		}else{
			resultLabel.setText("EMPTY SETTINGS");
		}
	}
	
	private SharedPreferences GetOtherAppSharedPreferences(){
		SharedPreferences settings = null; 
		try {
			Context otherAppCtx = this.getApplicationContext().createPackageContext(PACKAGE_NAME, Context.CONTEXT_IGNORE_SECURITY);
			settings = otherAppCtx.getSharedPreferences(PREFERENCES_FILE_NAME, Context.MODE_MULTI_PROCESS + Context.MODE_PRIVATE);
		} catch (NameNotFoundException e) {
			Log.v("NATIVE TEST", "The storage unit could not be accessed.");
		}
		return settings;
	}
	
	public void  launchAppverseShowcase(View v) {
		
		Log.v("NATIVE TEST", "launching appverse showcase...");
				
		Intent launchIntent = new Intent();
		String componentName = "com.gft.unity.poc.showcase/org.me.unity4jui_android.MainActivity";
		launchIntent.setComponent(ComponentName.unflattenFromString(componentName));
		
		EditText keyIntent = (EditText)findViewById(R.id.intentNameText);
		EditText keyValue = (EditText)findViewById(R.id.intentValueText);
		if(keyIntent!=null && keyIntent.getText()!=null && keyValue!=null && keyValue.getText()!=null) {
			launchIntent.putExtra(keyIntent.getText().toString(), keyValue.getText().toString());
			Log.v("NATIVE TEST", "adding intent extras: [" + keyIntent.getText().toString() +"]:" + keyValue.getText().toString());
		}
		
		this.startActivity(launchIntent);
	}

}
