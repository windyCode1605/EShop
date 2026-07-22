import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../my-lib/shared/enviroments/enviroment';

export interface UploadResponse {
  url: string;
}

@Injectable({
  providedIn: 'root'
})
export class UploadService {
  private readonly baseUrl = environment.api;

  constructor(private http: HttpClient) { }

  uploadImage(file: File, folder: string = 'products'): Observable<UploadResponse> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<UploadResponse>(`${this.baseUrl}/api/Upload/image?folder=${folder}`, formData);
  }
}
