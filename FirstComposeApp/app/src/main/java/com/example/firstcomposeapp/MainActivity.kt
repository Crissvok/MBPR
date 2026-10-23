package com.example.firstcomposeapp

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.material3.Button
import androidx.compose.material3.Scaffold
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.ui.Modifier
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import com.example.firstcomposeapp.ui.theme.FirstComposeAppTheme

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContent {
            FirstComposeAppTheme {
                Scaffold(modifier = Modifier.fillMaxSize()) { innerPadding ->
                    StudyProgress(
                        modifier = Modifier.padding(innerPadding)
                    )
                }
            }
        }
    }
}
@Composable
fun StudyProgress(modifier: Modifier = Modifier){
    val status = remember{
        mutableStateOf(0)
    }
    Column(modifier = modifier.fillMaxWidth().padding((20.dp))) {
        Text("Прогресс занятия")
        Spacer(modifier = Modifier.height(12.dp))
        Row {
            Text("Выполнено упражнений: ")
            Text("${status.value}")
        }
        Spacer(modifier = Modifier.height(12.dp))
        Button(onClick = {
            status.value = status.value + 1
        }) {
            Text("Упражнение выполнено")
        }
        Spacer(modifier = Modifier.height(12.dp))
        Button(onClick = {
            status.value = 0
        }) {
            Text("Сброс счета")
        }
    }
}
@Composable
fun UserProfile(modifier: Modifier = Modifier) {
    Column(modifier =modifier.fillMaxWidth().padding(20.dp)) {

        Text("Карточка пользователя")
        Spacer(modifier = Modifier.height(16.dp))
        Text("ФИО: Дюженко Дмитрий")
        Row() {
            Text("Группа: ")
            Text("ИСиП 23/1")
        }
        Row() {
            Text("Курс: ")
            Text("4")
        }
        Spacer(modifier = Modifier.height(8.dp))
        Row() {
            Text("Город: ")
            Text("Сургут")
        }
        Text("Специальность: Информационные системы и программирование")
        Row() {
            Text("Год: ")
            Text("2026")
        }
        Spacer(modifier = Modifier.height(12.dp))
        Button(onClick = {
            TODO()
        }) {
            Text("Продолжить")
        }
    }
}

@Preview(showBackground = true)
@Composable
fun StudyProgressPreview() {
    StudyProgress()
}


