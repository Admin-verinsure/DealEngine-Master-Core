/*
 *  Copyright 2013-2015 Vitaliy Fedorchenko
 *
 *  Licensed under PdfGenerator Source Code Licence (see LICENSE file).
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS 
 *  OF ANY KIND, either express or implied.
 */ 
#if NET_STANDARD
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NReco.PdfGenerator {
	

	internal static class NetStandardCompatibility {
		internal static void Close(this System.IO.StreamWriter wr) {
			wr.Dispose();
		} 
	}


}
#endif