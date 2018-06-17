pipeline {
  agent any
  stages {
    stage('Restore') {
      parallel {
        stage('Restore') {
          steps {
            sh 'dotnet restore'
          }
        }
        stage('Update Electron Packager') {
          steps {
            sh 'sudo npm install electron-packager -g'
          }
        }
        stage('[REMOVE]') {
          steps {
            sh 'whoami'
          }
        }
      }
    }
    stage('Build Server') {
      parallel {
        stage('Build Server') {
          steps {
            dir(path: './Server/') {
              sh 'dotnet publish -c Release -o ../Build/Server/'
            }

          }
        }
        stage('Build Client Win') {
          steps {
            dir(path: './Client/') {
              sh 'dotnet electronize build /target win'
            }

          }
        }
        stage('Build Client Linux') {
          steps {
            dir(path: './Client/') {
              sh 'dotnet electronize build /target linux'
            }

          }
        }
        stage('Build Client OSX') {
          steps {
            dir(path: './Client/') {
              sh '''
    
    
    
    dotnet electronize build /target linux'''
            }

          }
        }
      }
    }
  }
}