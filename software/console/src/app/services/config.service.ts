import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { LiteralsService } from './literals.service';

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
          console.log("Config data: ", data);
        })
        .catch(error =>
        {
          // Nothing to do
          console.log("Config data unchanged");
        });
      return promise;
  }

}
