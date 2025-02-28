﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public interface ISessionsRepository {

    Task<Session> getSavedSession();

    Task<Session> getSessionById(int id);

    Task<bool> saveSession(Session session);

    Task<List<Session>> getSessions();

    Task<bool> selectSession(Session session);

    Task<bool> clearAllSesstions(); 

}

