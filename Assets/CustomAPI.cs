public class CustomAPI {

	public static int StaticField = 42;

	public int MemberField = 91;

	int _privateField;

	public static int StaticIntMemberProperty {
		get; private set;
	}

	public int IntMemberProperty {
		get; private set;
	}

	public string StringMemberProperty {
		get; set;
	}

	public CustomAPI() {
	}

	public CustomAPI(string arg) {
	}

	public static void VoidStaticMethod() {
	}

	public static int IntReturningStaticMethod() {
		return 0;
	}

	static void PrivateStaticMethod() {
	}

	public void VoidMemberMethod() {
	}

	public string StringMemberMethod() {
		return string.Empty;
	}

	void PrivateMemberMethod() {
	}
}
