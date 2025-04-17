using System;

public interface ISocialAuthProvider
{
	string UserId { get; }
	bool IsReady { get; }
	event Action OnUserIdReady;
	void Initialize();
}
