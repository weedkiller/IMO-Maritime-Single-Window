import { Component, OnInit, OnChanges, Input, Output, EventEmitter, SimpleChanges } from '@angular/core';
import { ShipToShipActivityModel } from 'app/shared/models/ship-to-ship-activity-model';
import { LocalDataSource } from 'ng2-smart-table';
import { DeleteButtonComponent } from 'app/shared/components/delete-button/delete-button.component';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-ship-to-ship-activity-table',
  templateUrl: './ship-to-ship-activity-table.component.html',
  styleUrls: ['./ship-to-ship-activity-table.component.css'],
  providers: [DatePipe]
})
export class ShipToShipActivityTableComponent implements OnInit, OnChanges {
  @Input() shipToShipActivityList: ShipToShipActivityModel[] = [];
  @Output() delete = new EventEmitter<any>();

  shipActivitiesDataSource: LocalDataSource = new LocalDataSource();

  shipActivitiesTableSettings = {
    mode: 'external',
    actions: false,
    attr: {
      class: 'table table-bordered'
    },
    noDataMessage: 'No entries have been added yet.',

    columns: {
      location: {
        title: 'Location',
        type: 'html'
      },
      fromDate: {
        title: 'From Date',
        type: 'html',
        sortDirection: 'desc'
      },
      toDate: {
        title: 'To Date',
        type: 'html'
      },
      activityType: {
        title: 'Activity',
        type: 'html'
      },
      delete: {
        title: 'Delete',
        type: 'custom',
        filter: false,
        sort: false,
        renderComponent: DeleteButtonComponent,
        onComponentInitFunction: (instance) => {
          instance.delete.subscribe(row => {
            this.deleteShipActivity(row);
          });
        }
      }
    }
  };

  constructor(
    private datePipe: DatePipe
  ) { }

  ngOnInit() {
    if (this.shipToShipActivityList && this.shipToShipActivityList.length > 0) {
      const rows = this.generateRows();
      this.shipActivitiesDataSource.load(rows);
    } else {
      this.shipActivitiesDataSource.load([]);
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.shipToShipActivityList && this.shipToShipActivityList.length > 0) {
      const rows = this.generateRows();
      this.shipActivitiesDataSource.load(rows);
    } else {
      this.shipActivitiesDataSource.load([]);
    }
  }

  deleteShipActivity(row) {
    this.delete.emit(row);
  }

  generateRows() {
    const rowData = this.shipToShipActivityList.map(shipActivity => {
      return {
        shipToShipActivity: shipActivity,
        location: (shipActivity.location != null) ? shipActivity.location.name : '',
        fromDate: `<span class="no-wrap">` +
          this.datePipe.transform(shipActivity.fromDate, 'yyyy-MM-dd') +
          `</span> <span class="no-wrap">` +
          this.datePipe.transform(shipActivity.fromDate, 'HH:mm') +
          `</span>`,
        toDate: `<span class="no-wrap">` +
          this.datePipe.transform(shipActivity.toDate, 'yyyy-MM-dd') +
          `</span> <span class="no-wrap">` +
          this.datePipe.transform(shipActivity.toDate, 'HH:mm') +
          `</span>`,
        activityType: shipActivity.activityType.name,
      };
    });
    return rowData;
  }
}