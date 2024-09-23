import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { RegisterUser } from '../models/register-user';
import { Observable } from 'rxjs';
import { LoginUser } from '../models/login-user';

const API_BASE_URL = 'https://localhost:7009/api/v1/account/';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  public currentUserName: string | null = null;

  constructor(private httpClient: HttpClient) {}

  postRegister(registerUser: RegisterUser): Observable<any> {
    return this.httpClient.post<RegisterUser>(
      `${API_BASE_URL}register`,
      registerUser
    );
  }

  postLogin(loginUser: LoginUser): Observable<any> {
    return this.httpClient.post<any>(`${API_BASE_URL}login`, loginUser);
  }

  getLogout(): Observable<string> {
    return this.httpClient.get<string>(`${API_BASE_URL}logout`);
  }

  postGenerateNewToken(): Observable<any> {
    var token = localStorage['token'];
    var refreshToken = localStorage['refreshToken'];
    return this.httpClient.post<any>(`${API_BASE_URL}generate-new-jwt-token`, {
      token: token,
      refreshToken: refreshToken,
    });
  }
}
