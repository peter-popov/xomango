﻿/*
Copyright (C) 2009  Torgeir Helgevold

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using ExpressUnit;
using ExpressUnitViewModel;

namespace ExpressUnitGui
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool RunAllTests
        {
            get;
            set;
        }

        public static bool ConsoleMode
        {
            get;
            set;
        }

        public static TestMethodViewModel TestMethodViewModel
        {
            get;
            set;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ExpressUnitConfigurationSection config = (ExpressUnitConfigurationSection)System.Configuration.ConfigurationManager.GetSection("ExpressUnitConfiguration");
            RunAllTests = config.RunTestsOnStartup;

            if (e.Args.Length == 1)
            {
                ConsoleMode = true;
            }
            else
            {
                ConsoleMode = false;
            }
            
        }
    }
}
