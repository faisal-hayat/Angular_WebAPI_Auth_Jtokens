import { Injectable } from '@angular/core';
import { Login } from '../models/login';
import { Register } from '../models/register';
import { Observable } from "rxjs";
import { environment } from 'src/environments/environment.development';
import { JwtAuth } from '../models/jwtAuth';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  registerUrl = "Register";
  loginUrl = "Login";
  
  constructor(private http: HttpClient) {

  }

  public register(user: Register): Observable<JwtAuth>{

    return this.http.post<JwtAuth>(`${environment.apiUrl}/${this.registerUrl}`, user)
  }

  public login(user: Login): Observable<JwtAuth>{
    
    return this.http.post<JwtAuth>(`${environment.apiUrl}/${this.loginUrl}`, user)
  }
}