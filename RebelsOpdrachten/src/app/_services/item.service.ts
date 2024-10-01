import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { ItemResponse } from '../models/item';
import { environment } from '../../environments/environment';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ItemService {
    baseUrl: string = environment.apiUrl + 'items' 
    http = inject(HttpClient);

    getItems(lastEvaluatedKey?: string): Observable<ItemResponse> {
        let params = new HttpParams();
        if (lastEvaluatedKey) {
          params = params.set('lastEvaluatedKey', lastEvaluatedKey);
        }
        return this.http.get<ItemResponse>(this.baseUrl, { params }).pipe(tap((response) => {
            console.log(response)
        }));
      }
}
