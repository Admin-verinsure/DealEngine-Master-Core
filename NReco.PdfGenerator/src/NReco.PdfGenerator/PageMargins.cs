/*
 *  Copyright 2013-2017 Vitaliy Fedorchenko
 *
 *  Licensed under PdfGenerator Source Code Licence (see LICENSE file).
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS 
 *  OF ANY KIND, either express or implied.
 */ 
using System;
using System.Collections.Generic;
using System.Text;

namespace NReco.PdfGenerator {

	/// <summary>
	/// Represents PDF page margins (unit size is mm)
	/// </summary>
	public class PageMargins {

		/// <summary>
		/// Get or set top margin (in mm)
		/// </summary>
		public float? Top { get; set; }

		/// <summary>
		/// Get or set bottom margin (in mm)
		/// </summary>
		public float? Bottom { get; set; }

		/// <summary>
		/// Get or set left margin (in mm)
		/// </summary>
		public float? Left { get; set; }

		/// <summary>
		/// Get or set right margin (in mm)
		/// </summary>
		public float? Right { get; set; }
	}
}
