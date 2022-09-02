using Assets.Code.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PlayerPrefs = UnityEngine.PlayerPrefs;

namespace Assets.Code.utils {
    public class InputManager {

        private static ICacheProvider cahce = Injector.injectCahceProvider();

        public static void changeKey(Key key, KeyCode newKey) {
            cahce.putInt(key.ToString(), (int) newKey);
        }

        public static KeyCode geyKey(Key key) {
            int keyInt = cahce.getInt(key.ToString(), (int) key);
            return (KeyCode) keyInt;
        }
    }
    public enum Key {
        SPECIAL_FIRE = (int) KeyCode.Mouse0,
        BASIC_FIRE = (int) KeyCode.Mouse1,
        BOOST = (int) KeyCode.LeftShift,
        PAUSE = (int) KeyCode.Escape,
    }
}

public class Injector
{

    public static GameplayPresenter injectGameplayPresenter(GameplayView view) {
            return new GameplayPresenterImpl(view, injectGamePlayRepository(), injectAirplanesRepository(), injectSessionRepository());
        }

    public static IGamePlayRepository injectGamePlayRepository() {
            return GamePlayRepositoryImpl.getInstance(injectApiProvider(), injectCahceProvider());
        }

        public static IAirplanesRepository injectAirplanesRepository() {
            return AirplanesRepositoryImpl.getInstance(injectLocalStorageProivder(), injectCahceProvider(), injectJsonMapper());
        }

        public static ISessionsRepository injectSessionRepository() {
            return SessionsRepositoryImpl.getInstance(injectLocalStorageProivder(), injectCahceProvider(), injectJsonMapper());
        }

        public static IApiProvider injectApiProvider() {
            return new ApiProviderImpl(ApiEndPoints.BASE_URL, injectJsonMapper());
        }

        public static ICacheProvider injectCahceProvider() {
            return CacheProviderImpl.getInstance(JsonMapper.getInstance());
        }

        public static ILocalStorageProvider injectLocalStorageProivder() {
            return LocalStorageProviderImpl.getInstance();
        }

        public static JsonMapper injectJsonMapper() {
            return JsonMapper.getInstance();
        }
}
public class AppConstants {
    public const string REMOTE_URL = "http://192.168.43.92/squadron/";
    public const string REGISTER_URL = REMOTE_URL + "auth/register";
}
public class ApiEndPoints {

   
    public const string BASE_URL = AppConstants.REMOTE_URL + "api/";

    public const string LOGIN = "auth/login";

    public const string UPDATE = "user/update";

    public const string GET_RANKS = "ranks";

    public const string BUY_AIRPLANE = "user/buyAirplane";
}
public interface ICacheProvider {
    void putInt(String key, int value);

    void putFloat(string key, float value);

    void putString(string key, string value);

    void putBoolean(string key, bool value);

    void putObject<T>(string key, T value);

    int getInt(string key, int defaultValue = 0);

    float getFloat(string key, float defaultValue = 0.0f);

    string getString(string key, string defaultValue = "");

    bool getBoolean(string key, bool defaultValue = false);

    T getObject<T>(string key);

    void deleteKey(string key);
}

public class CacheProviderImpl : ICacheProvider {

    private static CacheProviderImpl INSTANCE;

    private JsonMapper jsonMapper;

    private CacheProviderImpl(JsonMapper jsonMapper) {
        this.jsonMapper = jsonMapper;
    }

    public static CacheProviderImpl getInstance(JsonMapper jsonMapper) {
        if (INSTANCE == null)
            INSTANCE = new CacheProviderImpl(jsonMapper);
        return INSTANCE;
    }

    public void deleteKey(string key) {
        PlayerPrefs.DeleteKey(key);
    }

    public bool getBoolean(string key, bool defaultValue = false) {
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    public float getFloat(string key, float defaultValue = 0) {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public int getInt(string key, int defaultValue = 0) {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public T getObject<T>(string key) {
        return jsonMapper.fromJson<T>(PlayerPrefs.GetString(key));
    }

    public string getString(string key, string defaultValue = "") {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public void putBoolean(string key, bool value) {
        PlayerPrefs.SetInt(key, value ? 1 : 0);
    }

    public void putFloat(string key, float value) {
        PlayerPrefs.SetFloat(key, value);
    }

    public void putInt(string key, int value) {
        PlayerPrefs.SetInt(key, value);
    }

    // Convert the model to string then save it in PlayerPrefs
    public void putObject<T>(string key, T value) {
        PlayerPrefs.SetString(key, jsonMapper.toJson(value));
    }

    public void putString(string key, string value) {
        PlayerPrefs.SetString(key, value);
    }
}


