namespace StartPos.Setup
{
    public static class MenuStartXmlSettingFile
    {
        public const string XmlBody = @"<?xml version=""1.0""?>
<Settings component=""StartMenu"" version=""4.3.0"">
	<MenuStyle value=""Classic1""/>
	<Documents value=""Hide""/>
	<UserFiles value=""Hide""/>
	<UserDocuments value=""Hide""/>
	<UserPictures value=""Hide""/>
	<ControlPanel value=""Hide""/>
	<Printers value=""Hide""/>
	<Shutdown value=""Menu""/>
	<LogOff value=""0""/>
	<Undock value=""0""/>
	<Search value=""0""/>
	<ShowAppsMenu value=""0""/>
	<Help value=""0""/>
	<Run value=""0""/>
	<HideProgramsMetro value=""0""/>
	<PinnedPrograms value=""PinnedItems""/>
	<RecentPrograms value=""None""/>
	<RecentMetroApps value=""0""/>
	<RecentProgsTop value=""0""/>
	<EnableJumplists value=""0""/>
	<RemoteShutdown value=""0""/>
	<HybridShutdown value=""0""/>
	<StartScreenShortcut value=""0""/>
	<HighlightNew value=""0""/>
	<CheckWinUpdates value=""1""/>
	<SearchTrack value=""1""/>
	<SearchResults value=""500""/>
	<SearchResultsMax value=""1000""/>
	<SearchAutoComplete value=""1""/>
	<SearchPrograms value=""1""/>
	<SearchFiles value=""1""/>
	<SearchInternet value=""0""/>
	<MainMenuAnimation value=""None""/>
	<SubMenuAnimation value=""None""/>
	<MenuCaption value=""StartPos - COMA""/>
	<MenuUsername value=""""/>
	<MenuShadow value=""0""/>
	<EnableGlass value=""0""/>
	<SkinC1 value=""Classic Skin""/>
	<SkinVariationC1 value=""""/>
	<SkinOptionsC1>
		<Line>CAPTION=1</Line>
		<Line>USER_IMAGE=0</Line>
		<Line>USER_NAME=0</Line>
		<Line>CENTER_NAME=0</Line>
		<Line>SMALL_ICONS=0</Line>
		<Line>THICK_BORDER=0</Line>
		<Line>SOLID_SELECTION=0</Line>
	</SkinOptionsC1>
	<EnableStartButton value=""0""/>
	<SkipMetro value=""1""/>
	<MenuItems1>
		<Line>Items=CustomItem6,CustomItem4,CustomItem2,CustomItem3,CustomItem,CustomItem5,CustomItem,ComputerItem,ControlPanelItem,ProgramsMenu,RunItem,SEPARATOR,RestartItem,ShutdownItem,RestartItem2,SearchMenu,SearchBoxItem</Line>
		<Line>CustomItem6.Command={0}</Line>
		<Line>CustomItem6.Label=PC-POS 7</Line>
		<Line>CustomItem6.Icon={1}\pcpos.ico</Line>
		<Line>CustomItem4.Command={0} DIAGNOSTIC</Line>
		<Line>CustomItem4.Label=Diagnostyka połaczenia z bazą danych</Line>
		<Line>CustomItem4.Icon={1}\postest.ico</Line>
		<Line>CustomItem2.Command={0} Repair</Line>
		<Line>CustomItem2.Label=Zestaw naprawczy</Line>
		<Line>CustomItem2.Icon={1}\repair.ico</Line>
		<Line>CustomItem3.Command={0} Setup</Line>
		<Line>CustomItem3.Label=StartPos Setup</Line>
		<Line>CustomItem3.Icon={1}\setup.ico</Line>
		<Line>CustomItem5.Command=%windir%\system32\osk.exe</Line>
		<Line>CustomItem5.Label=Klawiatura ekranowa</Line>
		<Line>CustomItem5.Icon=%windir%\system32\osk.exe, 1</Line>
		<Line>ComputerItem.Command=computer</Line>
		<Line>ControlPanelItem.Command=control</Line>
		<Line>ControlPanelItem.Label=Panel sterowania</Line>
		<Line>ControlPanelItem.Icon=shell32.dll,137</Line>
		<Line>ControlPanelItem.Settings=TRACK_RECENT</Line>
		<Line>ProgramsMenu.Command=programs</Line>
		<Line>ProgramsMenu.Label=$Menu.Programs</Line>
		<Line>ProgramsMenu.Tip=$Menu.ProgramsTip</Line>
		<Line>ProgramsMenu.Icon=shell32.dll,326</Line>
		<Line>ProgramsMenu.Settings=TRACK_RECENT</Line>
		<Line>RunItem.Command=run</Line>
		<Line>RunItem.Label=$Menu.Run</Line>
		<Line>RunItem.Tip=$Menu.RunTip</Line>
		<Line>RunItem.Icon=shell32.dll,328</Line>
		<Line>SearchBoxItem.Command=search_box</Line>
		<Line>SearchBoxItem.Label=$Menu.SearchBox</Line>
		<Line>SearchBoxItem.Icon=none</Line>
		<Line>SearchBoxItem.Settings=OPEN_UP|TRACK_RECENT</Line>
		<Line>RestartItem.Command={0} Restart</Line>
		<Line>RestartItem.Label=$Menu.Restart</Line>
		<Line>RestartItem.Tip=$Menu.RestartTip</Line>
		<Line>RestartItem.Icon={1}\restart.ico</Line>
		<Line>ShutdownItem.Command={0} Shutdown</Line>
		<Line>ShutdownItem.Label=$Menu.Shutdown</Line>
		<Line>ShutdownItem.Tip=$Menu.ShutdownTip</Line>
		<Line>ShutdownItem.Icon={1}\shutdown.ico</Line>
		<Line>RestartItem2.Command=restart</Line>
		<Line>RestartItem2.Label=Szybki restart - bez optymalizacji</Line>
		<Line>RestartItem2.Tip=$Menu.RestartTip</Line>
		<Line>RestartItem2.Icon={1}\restart.ico</Line>
		<Line>SearchMenu.Command=search</Line>
		<Line>SearchMenu.Label=$Menu.Search</Line>
		<Line>SearchMenu.Icon=shell32.dll,323</Line>
	</MenuItems1>
	<SkinA value=""&lt;Brak karnacji&gt;""/>
	<SkinVariationA value=""""/>
	<SkinOptionsA>
	</SkinOptionsA>
	<EnableContextMenu value=""1""/>
	<CascadingMenu value=""1""/>
	<EnableExit value=""1""/>
	<EnableExplorer value=""0""/>
</Settings>
";
    }
}