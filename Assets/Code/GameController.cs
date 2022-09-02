using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour,GameplayView
{
    [SerializeField]private GameObject TerrainLoad, AirplaneLoad;
    private GameplayPresenter presenter;

    private Session currentSession;
    private Airplane airplane;
    private Difficulty currentDifficulty;

    private GameState stateToSave; // use it to update the info after destroying the airplane

    private Terrain terrainObject;
    private GameObject airplaneObject;
    public Vector3 airplaneStartPosition = new Vector3(0, 50, 0);
    public GameObject finishMenu, connectoinErrorMenu;

    private Observer<AirplaneDeadEvent> airplaneDeadObserver;
    private Observer<EnemyDeadEvent> enemyDeadObserver;
    void Start() {
        presenter = Injector.injectGameplayPresenter(this);
        presenter.getCurrentSession();
    
    }
    private void OnDestroy() {
        EventBus<AirplaneDeadEvent>.getInstance().unregister(airplaneDeadObserver);
        EventBus<EnemyDeadEvent>.getInstance().unregister(enemyDeadObserver);
    }

    private void init() {
        presenter.getAirplane(currentSession.airplaneId);
        if (terrainObject == null) {
            terrainObject = Instantiate(TerrainLoad.GetComponent<Terrain>(), Vector3.zero, Quaternion.identity) as Terrain;
        }
        airplaneObject = Instantiate(AirplaneLoad, airplaneStartPosition, Quaternion.identity) as GameObject;
        airplaneObject.GetComponent<AirplaneManager>().setAirplane(airplane, currentSession.gameState);
        pushDifficulty((Difficulty) currentSession.gameState.difficultyLevel);
        airplaneDeadObserver = (airplane) => {
            AirplaneScore airplaneScore = airplaneObject.GetComponent<AirplaneScore>();
            stateToSave = new GameState(0f, currentSession.gameState.difficultyLevel, airplaneScore.coins, airplaneScore.score);
            presenter.update(stateToSave.score, stateToSave.coins);
        };
        enemyDeadObserver = (enemy) => {
            changeDifficulty();
        };
        EventBus<EnemyDeadEvent>.getInstance().register(enemyDeadObserver);
        EventBus<AirplaneDeadEvent>.getInstance().register(airplaneDeadObserver);
    }
    private void pushDifficulty(Difficulty difficulty) {
        this.currentDifficulty = difficulty;
        EventBus<DifficultyChangedEvent>.getInstance().publish(new DifficultyChangedEvent(difficulty));
    }
    public void saveGame() {
        Session getSession() {
            AirplaneManager airplaneManager = airplaneObject.GetComponent<AirplaneManager>();
            if (airplaneManager != null) {
                float health = airplaneManager.getCurrentHealth();
                int score = airplaneManager.getCurrentScore();
                Session session = new Session(currentSession.airplaneId, currentSession.environmentId, DateTime.Now.ToString(), new GameState(health, (int) currentDifficulty, 0, score));
                return session;
            }
            return null;
        }
        Session sessionToSave = getSession();
        presenter.saveSession(sessionToSave);
    }

    public void quitGame() {
        SceneManager.LoadScene("MainMenuScene");
    }
    private void changeDifficulty() {
        if (airplaneObject == null)
            return;
        int score = airplaneObject.GetComponent<AirplaneScore>().score;
        EventBus<DifficultyChangedEvent>.getInstance().publish(new DifficultyChangedEvent(DifficultyFactory.getDifficulty(score)));
    }
    // Called from presenter
    public void setAirplane(Airplane airplane) {
        this.airplane = airplane;
    }

    public void setCurrentSession(Session session) {
        this.currentSession = session;
        init();
    }

    public void sessoinSaved() {
        quitGame();
    }

    public void setUpdated(int score, int coines) {
        Cursor.visible = true;
        if (finishMenu != null && finishMenu.GetComponent<FinishMenu>() != null) {
            finishMenu.SetActive(true);
            finishMenu.GetComponent<FinishMenu>().setData(score, coines);
        } else
            quitGame();
    }

    public void setErrorConnection() {
        if (connectoinErrorMenu != null)
            connectoinErrorMenu.SetActive(true);
    }
}
public class DifficultyFactory {
    public static Difficulty getDifficulty(int score) {
        if (score >= 17) {
            return Difficulty.HIGH;
        } else if (score >= 10) {
            return Difficulty.MEDIUM;
        } else
            return Difficulty.LOW;
    }
}

class ResourcesPath {
    public const string AIRPLANS = "3D_Models\\airplanes\\";
    public const string ENVIRONMENTS = "Terrains\\";

    public static string AIRPLANES_FILE = Application.streamingAssetsPath + "\\files\\airplanes.json";
    public static string SESSIONS_FILE = Application.streamingAssetsPath + "\\files\\sessions.json";
    public static string ENVIRONMENTS_FILE = Application.streamingAssetsPath + "\\files\\environments.json";

    public const string AIRPLANES_IMAGES = "Images\\airplanes\\";
    public const string ENVIRONMENTS_IMAGES = "Images\\environments\\";
}