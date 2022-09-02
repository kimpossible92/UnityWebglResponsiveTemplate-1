using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
public interface GameplayView {

    void sessoinSaved();

    void setCurrentSession(Session session);

    void setAirplane(Airplane airplane);

    void setUpdated(int score, int coins);

    void setErrorConnection();
}

public interface GameplayPresenter {
    void getCurrentSession();

    void getAirplane(int id);

    void saveSession(Session session);

    void update(int score, int coins);
}
class GameplayPresenterImpl : GameplayPresenter {

    private GameplayView view;
    private IGamePlayRepository gamePlayRepository;
    private IAirplanesRepository airplanesRepository;
    private ISessionsRepository sessionRepository;

    private bool isLoading = false;

    public GameplayPresenterImpl(GameplayView view, IGamePlayRepository gamePlayRepository, IAirplanesRepository airplanesRepository, ISessionsRepository sessionRepository) {
        this.view = view;
        this.gamePlayRepository = gamePlayRepository;
        this.airplanesRepository = airplanesRepository;
        this.sessionRepository = sessionRepository;
    }

    public void getAirplane(int id) {
        Task<Airplane> task = airplanesRepository.getAirplaneById(id);
        task.Wait();
        view.setAirplane(task.Result);
    }

    public void getCurrentSession() {
        Task<Session> task = sessionRepository.getSavedSession();
        task.Wait();
        view.setCurrentSession(task.Result);
    }

    public void saveSession(Session session) {
        Task<bool> task = sessionRepository.saveSession(session);
        task.Wait();
        if (task.Result)
            view.sessoinSaved();
    }

    public void update(int score, int coins) {
        if (isLoading)
            return;
        isLoading = true;
        /*Task<bool> task = gamePlayRepository.update(score, coins);
        task.GetAwaiter().OnCompleted(() => {
            isLoading = false;
            if (task != null && task.Result) {
                view.setUpdated(score, coins);
            } else {
                view.setErrorConnection();
            }
        });*/
    }
}

