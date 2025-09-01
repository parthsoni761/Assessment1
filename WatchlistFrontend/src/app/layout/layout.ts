import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar'; // Or sidebar.ts
import { NavbarComponent } from '../navbar/navbar';
@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [RouterOutlet, SidebarComponent, NavbarComponent], // Add imports here
  templateUrl: './layout.html',
  styleUrls: ['./layout.css']
})
export class LayoutComponent { }
