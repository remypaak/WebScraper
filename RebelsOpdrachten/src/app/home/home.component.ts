import { Component, inject, OnInit } from '@angular/core';
import { ItemService } from '../_services/item.service';
import { AsyncPipe } from '@angular/common';
import { Item } from '../models/item';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [AsyncPipe],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit{
    itemService = inject(ItemService);
    items: Item[] = [];
    totalCount: number = 0;
    itemsPerPage: number = 9;


    ngOnInit(): void {
        this.itemService.getItems().subscribe(response => {
            this.items = response.items;
            this.totalCount = response.totalCount;
            this.calculatePages()
        })
    }

    calculatePages(): void {
        const totalPages = Math.ceil(this.totalCount / this.itemsPerPage);
        this.pages = [];
        for (let i = 1; i <= totalPages; i++) {
          this.pages.push(i);
        }
      }
}
