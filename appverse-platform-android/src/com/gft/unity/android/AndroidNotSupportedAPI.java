package com.gft.unity.android;

import com.gft.unity.core.beacon.IBeacon;
import com.gft.unity.core.system.SystemLogger;

public class AndroidNotSupportedAPI implements IBeacon{
	
	private static final SystemLogger LOG = AndroidSystemLogger.getInstance();

	@Override
	public void StartMonitoringAllRegions() {
		LOG.Log(SystemLogger.Module.PLATFORM, "Beacons not available under API lvl 18");		
	}

	@Override
	public void StartMonitoringRegion(String arg0) {
		LOG.Log(SystemLogger.Module.PLATFORM, "Beacons not available under API lvl 18");		
	}

	@Override
	public void StopMonitoringBeacons() {
		LOG.Log(SystemLogger.Module.PLATFORM, "Beacons not available under API lvl 18");		
	}

}
