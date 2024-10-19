import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ViewStatusService {

  constructor() { }

  public title: string = "No title";

  public setTitle(title: string)
  {
    this.title = title;
  }

}
