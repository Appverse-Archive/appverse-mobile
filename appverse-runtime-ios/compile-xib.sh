#!/bin/sh

nant -buildfile:../UnityBuild/unity_iOS_app_DEVICE_gftlabs.build -D:app.resources.dir=undefined -D:app.name=undefined compile-xib