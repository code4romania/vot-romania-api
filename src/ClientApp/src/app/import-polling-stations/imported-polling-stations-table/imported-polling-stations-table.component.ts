import { Component, OnInit, ViewChild, OnDestroy, AfterViewInit } from '@angular/core';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { Store, select } from '@ngrx/store';
import { MatPaginator } from '@angular/material/paginator';
import { Observable, merge, Subscription } from 'rxjs';
import { tap, map } from 'rxjs/operators';
import { ImportedPollingStation, ImportJobDetails, ImportedPollingStationsFilter } from 'src/app/services/data.service';
import { ApplicationState } from 'src/app/state/reducers';
import { LoadImportedPollingStationsAction, DeleteImportedPollingStationAction, UpdatePagination, ResetFilter, UpdateFilter } from 'src/app/state/actions';
import { getImportedPollingStations, getImportedPollingStationsTotal, getImportedPollingStationsLoading, getImportedPollingStationsError, getCurrentImportJobDetails, getCurrentImportedPollingStationsFilter as getImportedPollingStationsFilter } from 'src/app/state/selectors';
import { get } from 'lodash';

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
  public error$: Observable<{ hasError: boolean, errorMessage: string }>;
  public filter: ImportedPollingStationsFilter;
  private subscription: Subscription = new Subscription();
  currentJob: ImportJobDetails;

  constructor(public store: Store<ApplicationState>) { }

  public ngOnInit(): void {
    this.store.pipe(select(getImportedPollingStations)).subscribe(ps => this.initializeData(ps));
    this.store.pipe(select(getCurrentImportJobDetails)).subscribe(currentJob => {
      this.currentJob = currentJob;
    });

    this.store.pipe(select(getImportedPollingStationsFilter)).subscribe(filter => this.filter = filter);
    this.store.pipe(select(getImportedPollingStationsTotal)).subscribe(total => this.importedPollingStationsTotal = total);
    
    this.subscription.add(this.store.pipe(select(getImportedPollingStationsLoading)).subscribe(loading => {
      if (loading) {
        this.dataSource = new MatTableDataSource(this.noData);
      }
      this.loading = loading;
    }));

    this.error$ = this.store.pipe(select(getImportedPollingStationsError));
  }

  public ngAfterViewInit(): void {
    this.subscription.add(merge(this.paginator.page).pipe(
      map(()=>({
        pageNumber: get(this.paginator, 'pageIndex'), 
        pageSize: get(this.paginator, 'pageSize')
      })),
      tap(pagination => this.store.dispatch(new UpdatePagination(pagination)))
    ).subscribe());
  }
  updateFilter(){
    this.store.dispatch(new UpdateFilter(this.filter))
  }

  public resetFilter(): void {
    this.filter = {};
    this.store.dispatch(new ResetFilter());
  }


  deleteImportedPollingStation(pollingStation: ImportedPollingStation) {
    if (get(this.currentJob, 'jobId')) {
      this.store.dispatch(new DeleteImportedPollingStationAction(
        this.currentJob.jobId,
        pollingStation.id
      ));
    }
  }

  editImportedPollingStation() {

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
}
