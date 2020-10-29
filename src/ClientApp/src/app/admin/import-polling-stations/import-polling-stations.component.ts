import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { HttpEventType, HttpResponse } from '@angular/common/http';
import { ApplicationState } from 'src/app/state/reducers';
import { ImportJobDetails, PollingStationsService } from '../services/polling-stations.service';
import { LoadImportJobDetailsAction, RestartImportJobAction, FinishImportJobAction, CancelImportJobAction } from '../state/admin-actions';
import { getCurrentImportJobDetails } from '../state/admin-selectors';
import { SpinnerService } from '../services/spinner.service';

@Component({
  selector: 'app-import-polling-stations',
  templateUrl: './import-polling-stations.component.html',
  styleUrls: ['./import-polling-stations.component.scss']
})
export class ImportPollingStationsComponent implements OnInit, AfterViewInit {
  currentJob: ImportJobDetails;
  name = 'Angular';
  selectedFile: File
  progress: { percentage: string } = { percentage: '0%' }

  constructor(public store: Store<ApplicationState>,
    public pollingStationsService: PollingStationsService,
    private spinnerService: SpinnerService) { }

  ngOnInit() {
    this.store.pipe(select(getCurrentImportJobDetails)).subscribe(job => this.initializeData(job));
  }

  public ngAfterViewInit(): void {
    this.store.dispatch(new LoadImportJobDetailsAction());
  }

  initializeData(jobDetails: ImportJobDetails): void {
    this.currentJob = jobDetails;
  }

  restartCurrentJob(): void {
    this.store.dispatch(new RestartImportJobAction(this.currentJob.jobId));
  }

  finishImport(): void {
    this.store.dispatch(new FinishImportJobAction(this.currentJob.jobId));
  }

  cancelCurrentJob(): void {
    this.store.dispatch(new CancelImportJobAction(this.currentJob.jobId));
  }

  onFileSelected(event) {
    this.progress.percentage = '0%';
    this.selectedFile = event.target.files[0];
    this.onUpload();
  }

  onUpload() {
    this.spinnerService.display(true);
    this.pollingStationsService.uploadDocument(this.selectedFile)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          this.progress.percentage = Math.round(100 * event.loaded / event.total) + '%';
        } else if (event instanceof HttpResponse) {
          this.spinnerService.display(false);
          this.store.dispatch(new LoadImportJobDetailsAction());
        }
      }, error => {
        this.spinnerService.display(false);

        console.log(error);
      });
  }
}
