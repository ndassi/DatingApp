import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { errorMonitor } from 'events';
import { resourceUsage } from 'process';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  title = 'client';

  users:any;
  /**
   *
   */
  constructor(private http: HttpClient) {
    
    
  }
  ngOnInit(): void {
    this.http.get("https://localhost:7102/api/users").subscribe({
        next: respond =>{ console.log(respond); this.users = respond;},
        error : (error)=>{console.log(error)},
        complete:()=>{console.log("Request has completed !!!")}

    })
  }

}
