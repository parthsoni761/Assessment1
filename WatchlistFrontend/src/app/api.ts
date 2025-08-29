import { Injectable } from '@angular/core';

import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

@Injectable({

  providedIn: 'root'

})

export class ApiService {

  private baseUrl = 'https://localhost:7164/api'; // Replace with your backend's actual URL and port

  constructor(private http: HttpClient) { }

  // Example: Get data from a backend endpoint like /api/products

  getProducts(): Observable<any[]> {

    return this.http.get<any[]>(`${this.baseUrl}/products`); // Assuming you have a ProductController

  }

  // Example: Post data to a backend endpoint

  createProduct(product: any): Observable<any> {

    return this.http.post<any>(`${this.baseUrl}/products`, product);

  }

  // Add more methods for your other API endpoints (GET, POST, PUT, DELETE)

}
