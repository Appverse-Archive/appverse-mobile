
README

###########################
#### Appverse 5.0 - Module
###########################

To create an iOS Platform Module (iOS Library project like) you should:

	1. Create an iOS Library Project in the Xamarin iOS Studio:
		a. New Solution using the wizard menu: New Solution >> C# >> iOS >> Unified API >> iOS Library Project
		b. Close it and open sln file to make the folloing changes:
		
			i. Change the preSolution global section to include only the following configurations:
			
			GlobalSection(SolutionConfigurationPlatforms) = preSolution
				Debug|iPhoneSimulator = Debug|iPhoneSimulator
				Release|iPhoneSimulator = Release|iPhoneSimulator
				Debug|iPhone = Debug|iPhone
				Release|iPhone = Release|iPhone
			EndGlobalSection
			
			ii. Change the postSolution global section to include only the following configurations:
			
			GlobalSection(ProjectConfigurationPlatforms) = postSolution
				{6B1831F2-1152-4FC6-9A1B-90284D467F59}.Debug|iPhone.ActiveCfg = Debug|iPhone
				{6B1831F2-1152-4FC6-9A1B-90284D467F59}.Debug|iPhone.Build.0 = Debug|iPhone
				{6B1831F2-1152-4FC6-9A1B-90284D467F59}.Debug|iPhoneSimulator.ActiveCfg = Debug|iPhoneSimulator
				{6B1831F2-1152-4FC6-9A1B-90284D467F59}.Debug|iPhoneSimulator.Build.0 = Debug|iPhoneSimulator
				{6B1831F2-1152-4FC6-9A1B-90284D467F59}.Release|iPhone.ActiveCfg = Release|iPhone
				{6B1831F2-1152-4FC6-9A1B-90284D467F59}.Release|iPhone.Build.0 = Release|iPhone
				{6B1831F2-1152-4FC6-9A1B-90284D467F59}.Release|iPhoneSimulator.ActiveCfg = Release|iPhoneSimulator
				{6B1831F2-1152-4FC6-9A1B-90284D467F59}.Release|iPhoneSimulator.Build.0 = Release|iPhoneSimulator
			EndGlobalSection
			
			The UUID should be the one in the csproj project reference line:
			
			Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "Analytics", "Analytics\Analytics.csproj", "{6B1831F2-1152-4FC6-9A1B-90284D467F59}"
			
		c. Open the csproj file to make the following changes:
		
			i. Replace the "Any CPU" configuration with this ones:
			

		    <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|iPhoneSimulator' ">
		      <DebugType>none</DebugType>
		      <Optimize>False</Optimize>
		      <OutputPath>bin\iPhoneSimulator\Release</OutputPath>
		      <ErrorReport>prompt</ErrorReport>
		      <WarningLevel>4</WarningLevel>
		      <ConsolePause>False</ConsolePause>
		      <MtouchLink>None</MtouchLink>
		    </PropertyGroup>
		    <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|iPhone' ">
		      <DebugSymbols>True</DebugSymbols>
		      <DebugType>full</DebugType>
		      <Optimize>False</Optimize>
		      <OutputPath>bin\iPhone\Debug</OutputPath>
		      <DefineConstants>DEBUG</DefineConstants>
		      <ErrorReport>prompt</ErrorReport>
		      <WarningLevel>4</WarningLevel>
		      <MtouchDebug>True</MtouchDebug>
		      <ConsolePause>False</ConsolePause>
		      <CodesignKey>iPhone Developer</CodesignKey>
		      <PlatformTarget>x64</PlatformTarget>
		    </PropertyGroup>
		    <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|iPhone' ">
		      <DebugType>none</DebugType>
		      <Optimize>False</Optimize>
		      <OutputPath>bin\iPhone\Release</OutputPath>
		      <ErrorReport>prompt</ErrorReport>
		      <WarningLevel>4</WarningLevel>
		      <ConsolePause>False</ConsolePause>
		      <CodesignKey>iPhone Developer</CodesignKey>
		    </PropertyGroup>
		
		d. After all changes, you could open again the solution and start the module implementation
			
			i. If the module needs third-party libraries please place them in a "lib" folder at the project folder
			
	2. Create a "module.properties" file at the Solution root folder:
		
		a. The properties to be included for each module are (the following is an example):
		
			 # common module properties
			 module.name=Analytics
			 module.api.service.name=analytics

			 # ios module properties
			 module.ios.main.class=Appverse.Platform.IPhone.IPhoneAnalytics
			 module.ios.lib.folder=lib
			 # dll references comma separated
			 module.ios.lib.references=GoogleAnalytics.iOS.dll
			
	3. Create a "module.js" file at the Solution root folder, containing the javascript interfaces to be added to the appverse.js. 
			
			The namescape of the API could be added to the Appverse object, or you could define another custom one just for you app.
			Appverse Core Modules are defined by the Appverse namespace.
			
			 
			 
	4. When uploading to repository, "revert" bin and obj folder and then "ignore" them.
			
			
		