﻿/*
    Copyright (C) 2011-2012 de4dot@gmail.com

    This file is part of de4dot.

    de4dot is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    de4dot is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with de4dot.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using DeMono.Cecil;

namespace de4dot.code.deobfuscators.dotNET_Reactor.v3 {
	class DecryptMethod {
		MethodDefinition decryptionMethod;
		byte[] key;
		byte[] iv;

		public byte[] Key {
			get { return key; }
		}

		public byte[] Iv {
			get { return iv; }
		}

		public bool Detected {
			get { return decryptionMethod != null; }
		}

		public static bool couldBeDecryptMethod(MethodDefinition method, IEnumerable<string> additionalTypes) {
			if (method.Body == null)
				return false;

			var localTypes = new LocalTypes(method);
			var requiredTypes = new List<string> {
				"System.Byte[]",
				"System.IO.MemoryStream",
				"System.Security.Cryptography.CryptoStream",
				"System.Security.Cryptography.ICryptoTransform",
			};
			requiredTypes.AddRange(additionalTypes);
			if (!localTypes.all(requiredTypes))
				return false;
			if (!localTypes.exists("System.Security.Cryptography.RijndaelManaged") &&
				!localTypes.exists("System.Security.Cryptography.AesManaged"))
				return false;

			return true;
		}

		public bool getKey(MethodDefinition method) {
			var tmpKey = ArrayFinder.getInitializedByteArray(method, 32);
			if (tmpKey == null)
				return false;
			var tmpIv = ArrayFinder.getInitializedByteArray(method, 16);
			if (tmpIv == null)
				return false;

			decryptionMethod = method;
			key = tmpKey;
			iv = tmpIv;
			return true;
		}
	}
}
