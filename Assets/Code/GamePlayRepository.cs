using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface IGamePlayRepository {

    void startNewGame(int airplaneId, int environemtId);

    Task<List<Difficulty>> getDifficulties();

    void changeDifficulty(Difficulty difficulty);

    Difficulty getSavedDifficulty();

    //Task<bool> update(int newScore, int newCoins);

}
public class GamePlayRepositoryImpl : IGamePlayRepository {

    private static GamePlayRepositoryImpl INSTANCE;

    private IApiProvider apiProvider;
    private ICacheProvider cache;

    private GamePlayRepositoryImpl(IApiProvider apiProvider, ICacheProvider cache) {
        this.apiProvider = apiProvider;
        this.cache = cache;
    }

    public static GamePlayRepositoryImpl getInstance(IApiProvider apiProvider, ICacheProvider cache) {
        if (INSTANCE == null)
            INSTANCE = new GamePlayRepositoryImpl(apiProvider, cache);
        return INSTANCE;
    }

    public void startNewGame(int airplaneId, int environemtId) {
        cache.putInt(CacheKeys.AIRPLANE_ID, airplaneId);
        cache.putInt(CacheKeys.ENVIRONMENT_ID, environemtId);
        cache.deleteKey(CacheKeys.SAVED_SESSION_ID);
    }

    public Task<List<Difficulty>> getDifficulties() {
        return Task.Run(() => {
            return Enum.GetValues(typeof(Difficulty)).OfType<Difficulty>().ToList();
        });
    }

    public void changeDifficulty(Difficulty difficulty) {
        cache.putInt(CacheKeys.DIFFICULTY, (int) difficulty);
    }

    public Difficulty getSavedDifficulty() {
        int difficultyValue = cache.getInt(CacheKeys.DIFFICULTY, 1);
        return (Difficulty) difficultyValue;
    }

    /*public Task<bool> update(int newScore, int newCoins) {
        // Check if the user is logged-in
        UserState userState = (UserState) cache.getInt(CacheKeys.USER_STATE, (int) UserState.GUEST);
        if (userState == UserState.GUEST) {
            return Task.Run(() => true);
        }
        // Fetch userId from the cache
        int userId = cache.getObject<User>(CacheKeys.USER).id;

        // Make a request to api
        Task<UpdateResponse> task = apiProvider.post<UpdateResponse>(ApiEndPoints.UPDATE, new UpdateRequest(userId, newScore, newCoins));
        task.GetAwaiter().OnCompleted(() => {
            if (task != null && task.Result != null && task.Result.success) {
                try {
                    cache.putObject<User>(CacheKeys.USER, task.Result.data.user);
                } catch (Exception _) { }
            }
        });

        // Mapping the response task to task of boolean
        return task.ContinueWith(returnedTask => {
            return returnedTask != null && returnedTask.Result != null && returnedTask.Result.success;
        });
    }*/
}
public class CacheKeys {
    public const string AIRPLANE_ID = "airplane_id"; // int
    public const string ENVIRONMENT_ID = "environment_id"; // int
    public const string DIFFICULTY = "difficulty"; // int (enum value)
    public const string SAVED_SESSION_ID = "saved_session_id"; // int
    public const string IS_LOGGED_IN = "is_logged_in"; // bool
    public const string USER_STATE = "user_state"; // int
    public const string USER = "user"; // object (User)
    public const string TOKEN = "token"; // string
}