import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AngularEditorModule } from '@kolkov/angular-editor';
import { MatInputModule, MatDialogModule } from '@angular/material';
import { LoginComponent } from './login/login.component';
import { AdminComponent } from './admin.component';
import { AdminContentComponent } from './admin-content/admin-content.component';
import { VotersOptionEditorComponent } from './admin-content/voters-option-editor/voters-option-editor.component';
import { AdminRoutingModule } from './admin-routing.module';
import { DataService } from './../services/data.service';


@NgModule({
  declarations: [
    LoginComponent,
    AdminComponent,
    AdminContentComponent,
    VotersOptionEditorComponent
  ],
  entryComponents: [
    VotersOptionEditorComponent
  ],
  imports: [
    CommonModule,
    AdminRoutingModule,
    FormsModule,
    MatInputModule,
    MatDialogModule,
    ReactiveFormsModule,
    AngularEditorModule,
  ],
   providers: [
    DataService
  ],
})
export class AdminModule { }
