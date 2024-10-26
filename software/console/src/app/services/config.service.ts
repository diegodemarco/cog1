import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { BasicEntitiesService } from './basic-entities.service';

class ConfigData
{
  public apiBasePath: string = "";
}

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  public apiBasePath: string = "";

	constructor(private http: HttpClient) {}

		load(): Promise<any>  
    {
      const promise = firstValueFrom(this.http.get<ConfigData>('assets/app.config.json'))
        .then(data => {
          this.apiBasePath = data.apiBasePath;
        })
        .catch(error =>
        {
          // No configuration file, nothing to do
        });
      return promise;
  }

}
