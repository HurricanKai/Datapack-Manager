pipeline {
  agent any
  stages {
    stage('Restore') {
      parallel {
        stage('Restore') {
          steps {
            sh 'dotnet clean'
            sh 'dotnet restore'
            dir(path: './Client/') {
              sh 'dotnet restore'
            }

            dir(path: './Server/') {
              sh 'dotnet restore'
            }

          }
        }
        stage('Update Electron Packager') {
          steps {
            sh 'sudo npm install electron-packager -g'
          }
        }
      }
    }
    stage('Build') {
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
              sh 'dotnet electronize build /target win -f netcoreapp2.0 -o ../Build/Client/win'
            }

          }
        }
      }
    }
    stage('Build Linux') {
      steps {
        dir(path: './Client/') {
          sh 'dotnet electronize build /target linux -f netcoreapp2.0 -o ../Build/Client/linux'
        }

      }
    }
    stage('Pack') {
      parallel {
        stage('Pack Clients') {
          steps {
            archiveArtifacts(artifacts: './Client/bin/desktop/win/**/*', onlyIfSuccessful: true)
            archiveArtifacts(artifacts: './Client/bin/desktop/linux/**/*', onlyIfSuccessful: true)
          }
        }
        stage('Pack Server') {
          steps {
            archiveArtifacts(artifacts: '/Build/Server/**/*', onlyIfSuccessful: true)
            dir(path: './Build/')
          }
        }
      }
    }
  }
}