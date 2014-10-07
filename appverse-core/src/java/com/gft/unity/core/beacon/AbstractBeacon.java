/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gft.unity.core.beacon;

/**
 *
 * @author FRMI
 */
public abstract class AbstractBeacon implements IBeacon{

    public AbstractBeacon(){
        
    }
    
    @Override
    public void StartMonitoringAllRegions() {
       StartMonitoringRegion(null);
        
    }
        
    @Override
    public abstract void StartMonitoringRegion(String UUID);

    @Override
    public abstract void StopMonitoringBeacons();
    
}
