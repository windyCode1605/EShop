import { HttpClient } from "@angular/common/http";
import { Injectable, signal } from "@angular/core";
import { ICategory } from "../../../core/models/category.model";
import { environment } from "../../../my-lib/shared/enviroments/enviroment";
import { API_ENDPOINTS } from "../../../core/constants/api-endpoints.const";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class CategoryService {
    categories = signal<ICategory[]>([]);
    constructor(private http: HttpClient) { }
    loadCategory() {
        if (this.categories().length > 0) return;

        this.http.get<any>(API_ENDPOINTS.CATEGORY.GET_ALL)
            .subscribe(response => {
                if (response.success && response.data) {
                    this.categories.set(response.data);
                }
            });
    }
}