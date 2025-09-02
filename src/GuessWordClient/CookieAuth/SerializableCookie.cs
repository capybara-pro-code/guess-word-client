// ReSharper disable MemberCanBePrivate.Global

namespace GuessWordClient.CookieAuth;

public class SerializableCookie {
	public string Name { get; set; } = null!;
	public string Value { get; set; } = null!;
	public string Domain { get; set; } = null!;
	public string Path { get; set; } = null!;
	public DateTime Expires { get; set; }
	public bool Secure { get; set; }
	public bool HttpOnly { get; set; }

	// ReSharper disable once UnusedMember.Global
	public SerializableCookie() { }

	public SerializableCookie(Cookie cookie) {
		Name = cookie.Name;
		Value = cookie.Value;
		Domain = cookie.Domain;
		Path = cookie.Path;
		Expires = cookie.Expires;
		Secure = cookie.Secure;
		HttpOnly = cookie.HttpOnly;
	}

	public Cookie ToCookie() {
		return new Cookie(Name, Value, Path, Domain) {
			Expires = Expires,
			Secure = Secure,
			HttpOnly = HttpOnly
		};
	}
}
