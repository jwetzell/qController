namespace SharpOSC
{
	public struct RGBA
	{
		public byte Red;
		public byte Green;
		public byte Blue;
		public byte Alpha;

		public RGBA(byte red, byte green, byte blue, byte alpha)
		{
			this.Red = red;
			this.Green = green;
			this.Blue = blue;
			this.Alpha = alpha;
		}

		public override bool Equals(System.Object obj)
		{
			if (obj.GetType() == typeof(RGBA))
			{
				if (this.Red == ((RGBA)obj).Red && this.Green == ((RGBA)obj).Green && this.Blue == ((RGBA)obj).Blue && this.Alpha == ((RGBA)obj).Alpha)
					return true;
				else
					return false;
			}
			else if (obj.GetType() == typeof(byte[]))
			{
				if (this.Red == ((byte[])obj)[0] && this.Green == ((byte[])obj)[1] && this.Blue == ((byte[])obj)[2] && this.Alpha == ((byte[])obj)[3])
					return true;
				else
					return false;
			}
			else
				return false;
		}

		public static bool operator ==(RGBA a, RGBA b)
		{
			if (a.Equals(b))
				return true;
			else
				return false;
		}

		public static bool operator !=(RGBA a, RGBA b)
		{
			if (!a.Equals(b))
				return true;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return (Red << 24) + (Green << 16) + (Blue << 8) + (Alpha);
		}
	}
}
