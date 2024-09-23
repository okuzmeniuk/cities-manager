import { Injectable } from '@angular/core';
import { City } from '../models/city';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

const API_BASE_URL = 'https://localhost:7009/api/';

@Injectable({
  providedIn: 'root',
})
export class CitiesService {
  cities: City[] = [];
  constructor(private httpClient: HttpClient) {}

  getCities(): Observable<City[]> {
    const headers = new HttpHeaders().append(
      'Authorization',
      `Bearer ${localStorage['token']}`
    );
    return this.httpClient.get<City[]>(`${API_BASE_URL}v1/Cities`, {
      headers: headers,
    });
  }

  postCity(city: City): Observable<City> {
    const headers = new HttpHeaders().append(
      'Authorization',
      `Bearer ${localStorage['token']}`
    );
    return this.httpClient.post<City>(`${API_BASE_URL}v1/Cities`, city, {
      headers: headers,
    });
  }

  putCity(city: City): Observable<string> {
    const headers = new HttpHeaders().append(
      'Authorization',
      `Bearer ${localStorage['token']}`
    );
    return this.httpClient.put<string>(
      `${API_BASE_URL}v1/Cities/${city.cityID}`,
      city,
      {
        headers: headers,
      }
    );
  }

  deleteCity(cityId: string | null): Observable<string> {
    const headers = new HttpHeaders().append(
      'Authorization',
      `Bearer ${localStorage['token']}`
    );
    return this.httpClient.delete<string>(
      `${API_BASE_URL}v1/Cities/${cityId}`,
      {
        headers: headers,
      }
    );
  }
}
