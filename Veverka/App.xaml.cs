﻿using Veverka.Services;

namespace Veverka;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
		
	}

}
