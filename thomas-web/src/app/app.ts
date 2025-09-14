import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { JsonPipe, NgIf } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet,JsonPipe, NgIf],  
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('thomas-web');
  private http = inject(HttpClient);
  data: any;

  constructor() {
    const api = environment.apiBaseUrl;

    this.http.get(`${api}/health`).subscribe(r => 
      {
        (this.data = r);
        console.log(r);
      });
  }
}
