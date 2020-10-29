import { AfterViewInit, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { select, Store } from '@ngrx/store';
import { get } from 'lodash';
import { merge, Subscription } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { ApplicationState } from 'src/app/state/reducers';

import { PollingStationEditorComponent } from '../../imported-polling-station-editor/polling-station-editor.component';
import {
  ImportedPollingStation,
  ImportedPollingStationsFilter,
  ImportJobDetails,
} from '../../services/polling-stations.service';
import {
  DeleteImportedPollingStationAction,
  LoadImportedPollingStationsAction,
  ResetFilter,
  UpdateFilter,
  UpdatePagination,
} from '../../state/admin-actions';
import {
  getCurrentImportedPollingStationsFilter,
  getCurrentImportJobDetails,
  getImportedPollingStations,
  getImportedPollingStationsTotal,
} from '../../state/admin-selectors';

@Component({
  selector: 'app-imported-polling-stations-table',
  templateUrl: './imported-polling-stations-table.component.html',
  styleUrls: ['./imported-polling-stations-table.component.scss']
})
export class ImportedPollingStationsTableComponent implements OnInit, OnDestroy, AfterViewInit {

  @ViewChild(MatSort, { static: false }) sort: MatSort;
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  public displayedColumns: string[] = [
    'county',
    'locality',
    'pollingStationNumber',
    'address',
    'institution',
    'resolvedAddressStatus',
    'failMessage',
    'actions'
  ];


  public dataSource: MatTableDataSource<ImportedPollingStation>;
  public importedPollingStationsTotal: number;
  public noData: ImportedPollingStation[] = [];
  public loading: boolean;
  public filter: ImportedPollingStationsFilter;
  private subscription: Subscription = new Subscription();
  currentJob: ImportJobDetails;

  constructor(public store: Store<ApplicationState>,
    public dialog: MatDialog) { }

  public ngOnInit(): void {
    this.store.pipe(select(getImportedPollingStations)).subscribe(ps => this.initializeData(ps));
    this.store.pipe(select(getCurrentImportJobDetails)).subscribe(currentJob => {
      this.currentJob = currentJob;
    });

    this.store.pipe(select(getCurrentImportedPollingStationsFilter)).subscribe(filter => this.filter = filter);
    this.store.pipe(select(getImportedPollingStationsTotal)).subscribe(total => this.importedPollingStationsTotal = total);

  }

  public ngAfterViewInit(): void {
    this.subscription.add(merge(this.paginator.page).pipe(
      map(() => ({
        pageNumber: get(this.paginator, 'pageIndex', 0) + 1,
        pageSize: get(this.paginator, 'pageSize', 5)
      })),
      tap(pagination => this.store.dispatch(new UpdatePagination(pagination)))
    ).subscribe());
  }

  updateFilter() {
    this.paginator.firstPage();
    this.store.dispatch(new UpdateFilter(this.filter));
  }

  public resetFilter(): void {
    this.paginator.firstPage();
    this.filter = {};
    this.store.dispatch(new ResetFilter());
  }

  deleteImportedPollingStation(pollingStation: ImportedPollingStation) {
    if (confirm('Are you sure you want to delete this polling station?')) {
      this.store.dispatch(new DeleteImportedPollingStationAction(
        this.currentJob.jobId,
        pollingStation.id
      ));
    }
  }

  editImportedPollingStation(pollingStation: ImportedPollingStation) {
    this.dialog.open(PollingStationEditorComponent, {
      data: {
        pollingStation: pollingStation,
        jobId: this.currentJob.jobId,
      }
    });


  }

  private initializeData(pollingStations: ImportedPollingStation[]): void {
    this.dataSource = new MatTableDataSource(pollingStations.length ? pollingStations : this.noData);
  }

  public ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  public retry(): void {
    this.store.dispatch(new LoadImportedPollingStationsAction());
  }

  public addNewPollingStation(): void {
    this.dialog.open(PollingStationEditorComponent, { data: { jobId: this.currentJob.jobId } });
  }

}
