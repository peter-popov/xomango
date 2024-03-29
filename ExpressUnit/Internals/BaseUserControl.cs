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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;
using ExpressUnit;

namespace ExpressUnitGui
{
    public partial class BaseUserControl : UserControl
    {
        protected void SetTextBlockFromWorkerThread(TextBlock textBlock, string msg)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Action(
                        delegate()
                        {
                            textBlock.Text = msg;
                        }
                        ));
        }

        protected void SetTreeNodeColor(TestMethod node, string color)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal,
                 new Action(
                     delegate()
                     {
                         node.Color = color;
                     }
                     ));
        }
    }
}
