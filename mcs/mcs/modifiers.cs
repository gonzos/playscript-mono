//
// modifiers.cs: Modifier handling.
// 
using System;

namespace CIR {
	public class Modifiers {

		//
		// The ordering of the following 4 constants
		// has been carefully done.
		//
		public const int PROTECTED = 0x0001;
		public const int PUBLIC    = 0x0002;
		public const int PRIVATE   = 0x0004;
		public const int INTERNAL  = 0x0008;
		public const int NEW       = 0x0010;
		public const int ABSTRACT  = 0x0020;
		public const int SEALED    = 0x0040;
		public const int STATIC    = 0x0080;
		public const int READONLY  = 0x0100;
		public const int VIRTUAL   = 0x0200;
		public const int OVERRIDE  = 0x0400;
		public const int EXTERN    = 0x0800;
		public const int TOP       = 0x0800;

		public const int Accessibility =
			PUBLIC | PROTECTED | INTERNAL | PRIVATE;
		
		static public string Name (int i)
		{
			string s = "";
			
			switch (i) {
			case Modifiers.NEW:
				s = "new"; break;
			case Modifiers.PUBLIC:
				s = "public"; break;
			case Modifiers.PROTECTED:
				s = "protected"; break;
			case Modifiers.INTERNAL:
				s = "internal"; break;
			case Modifiers.PRIVATE:
				s = "private"; break;
			case Modifiers.ABSTRACT:
				s = "abstract"; break;
			case Modifiers.SEALED:
				s = "sealed"; break;
			case Modifiers.STATIC:
				s = "static"; break;
			case Modifiers.READONLY:
				s = "readonly"; break;
			case Modifiers.VIRTUAL:
				s = "virtual"; break;
			case Modifiers.OVERRIDE:
				s = "override"; break;
			case Modifiers.EXTERN:
				s = "extern"; break;
			}

			return s;
		}
		
		// <summary>
		//   Checks the object @mod modifiers to be in @allowed.
		//   Returns the new mask.  Side effect: reports any
		//   incorrect attributes. 
		// </summary>
		public static int Check (int allowed, int mod, int def_access)
		{
			int invalid_flags  = (~allowed) & mod;
			int i;

			if (invalid_flags == 0){
				int a = mod;

				//
				// If no accessibility bits provided
				// then provide the defaults.
				//
				if ((mod & Accessibility) == 0){
					mod |= def_access;
					return mod;
				}

				//
				// Make sure that no conflicting accessibility
				// bits have been set.  Protected+Internal is
				// allowed, that is why they are placed on bits
				// 1 and 4 (so the shift 3 basically merges them)
				//
				a &= 15;
				a |= (a >> 3);
				a = ((a & 2) >> 1) + (a & 5);
				a = ((a & 4) >> 2) + (a & 3);
				if (a > 1)
					CSC.CSharpParser.error (107, "More than one protecion modifier specified");
				
				return mod;
			}
			
			for (i = 1; i < TOP; i <<= 1){
				if ((i & invalid_flags) == 0)
					continue;

				CSC.CSharpParser.error (106, "the modifier `" + Name (i) + "' is not valid for this item");
			}

			return allowed & mod;
		}
	}
}
