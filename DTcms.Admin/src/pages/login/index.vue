<template>
	<div class="loginbody">
		<div class="bg-blur"></div>
		<div class="login-box">
			<el-card>
				<div class="title">DTcms管理后台</div>
				<div class="logo"></div>
				<el-form ref="loginForm" :model="datas.model" :rules="datas.rules" @submit.native.prevent>
					<el-form-item prop="username">
						<el-input type="text" v-model="datas.model.username" placeholder="请输入账户名" prefix-icon="User"></el-input>
					</el-form-item>
					<el-form-item prop="password">
						<el-input type="password" v-model="datas.model.password" placeholder="请输入密码" prefix-icon="Lock"></el-input>
					</el-form-item>
					<el-form-item prop="codeValue">
						<el-input type="text" v-model="datas.model.codeValue" placeholder="请输入验证码" prefix-icon="Postcard">
							<template #append>
								<el-image @click="toggleCode" class="code" :src="datas.imgSrc" alt="点击切换验证码" />
							</template>
						</el-input>
					</el-form-item>
					<el-form-item>
		                <el-button :loading="datas.btnLoading" type="primary" native-type="submit" @click="submitForm">登 录</el-button>
					</el-form-item>
				</el-form>
			</el-card>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from 'vue'
	import { useUserStore } from '../../stores/userStore.js'
	
	const { proxy } = getCurrentInstance()
	const userStore  = useUserStore()
	const loginForm = ref(null)
	
	const datas = reactive({
		btnLoading: false,
		imgSrc: '',
		model: {
			username: 'admin',
			password: 'admin888',
			codeKey: null,
			codeValue: null,
		},
		rules: {
			username: [
				{ required: true, message: '请输入登录用户名', trigger: 'blur' },
				{ min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' },
			],
			password: [
				{ required: true, message: '请输入登录密码', trigger: 'blur' },
				{ min: 6, max: 20, message: '长度在 6 到 20 个字符', trigger: 'blur' },
			],
			codeValue: [
				{ required: true, message: '请输入验证码', trigger: 'blur' },
				{ len: 4, message: '请输入 4 位的验证码', trigger: 'blur' },
			],
		},
	})
	//初始化数据
	const initData = () => {
		proxy.$api.request({
			url: '/verifycode',
			success(res) {
				datas.model.codeKey = res.data.key
				datas.imgSrc = res.data.data
			}
		});
	}
	//提交表单
	const submitForm = (formName) => {
		if (!loginForm.value) return
		//表单验证
		loginForm.value.validate((valid) => {
			if (valid) {
				proxy.$api.request({
					method: 'post',
					url: '/auth/login',
					data: datas.model,
					beforeSend() {
						datas.btnLoading = true
					},
					success(res) {
						//保存Token，以及跳转
						let tokenObj = {'accessToken': res.data.accessToken, 'refreshToken': res.data.refreshToken};
						userStore.login(tokenObj);
					},
					complete() {
						datas.btnLoading = false
					}
				})
			} else {
				return false;
			}
		})
	}
	//切换验证码
	const toggleCode = () => {
		initData()
	}
	
	//初始化数据
	initData()
	
</script>

<style lang="scss">
	.loginbody {
		background-color: #0E70D5;
		background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='100%25' height='100%25' viewBox='0 0 1600 800'%3E%3Cg fill-opacity='1'%3E%3Cpath fill='%230e72d8'  d='M486 705.8c-109.3-21.8-223.4-32.2-335.3-19.4C99.5 692.1 49 703 0 719.8V800h843.8c-115.9-33.2-230.8-68.1-347.6-92.2C492.8 707.1 489.4 706.5 486 705.8z'/%3E%3Cpath fill='%230e73dc'  d='M1600 0H0v719.8c49-16.8 99.5-27.8 150.7-33.5c111.9-12.7 226-2.4 335.3 19.4c3.4 0.7 6.8 1.4 10.2 2c116.8 24 231.7 59 347.6 92.2H1600V0z'/%3E%3Cpath fill='%230f75df'  d='M478.4 581c3.2 0.8 6.4 1.7 9.5 2.5c196.2 52.5 388.7 133.5 593.5 176.6c174.2 36.6 349.5 29.2 518.6-10.2V0H0v574.9c52.3-17.6 106.5-27.7 161.1-30.9C268.4 537.4 375.7 554.2 478.4 581z'/%3E%3Cpath fill='%230f76e2'  d='M0 0v429.4c55.6-18.4 113.5-27.3 171.4-27.7c102.8-0.8 203.2 22.7 299.3 54.5c3 1 5.9 2 8.9 3c183.6 62 365.7 146.1 562.4 192.1c186.7 43.7 376.3 34.4 557.9-12.6V0H0z'/%3E%3Cpath fill='%230F78E5'  d='M181.8 259.4c98.2 6 191.9 35.2 281.3 72.1c2.8 1.1 5.5 2.3 8.3 3.4c171 71.6 342.7 158.5 531.3 207.7c198.8 51.8 403.4 40.8 597.3-14.8V0H0v283.2C59 263.6 120.6 255.7 181.8 259.4z'/%3E%3Cpath fill='%230f76e0'  d='M1600 0H0v136.3c62.3-20.9 127.7-27.5 192.2-19.2c93.6 12.1 180.5 47.7 263.3 89.6c2.6 1.3 5.1 2.6 7.7 3.9c158.4 81.1 319.7 170.9 500.3 223.2c210.5 61 430.8 49 636.6-16.6V0z'/%3E%3Cpath fill='%230e73dc'  d='M454.9 86.3C600.7 177 751.6 269.3 924.1 325c208.6 67.4 431.3 60.8 637.9-5.3c12.8-4.1 25.4-8.4 38.1-12.9V0H288.1c56 21.3 108.7 50.6 159.7 82C450.2 83.4 452.5 84.9 454.9 86.3z'/%3E%3Cpath fill='%230e71d7'  d='M1600 0H498c118.1 85.8 243.5 164.5 386.8 216.2c191.8 69.2 400 74.7 595 21.1c40.8-11.2 81.1-25.2 120.3-41.7V0z'/%3E%3Cpath fill='%230d6fd2'  d='M1397.5 154.8c47.2-10.6 93.6-25.3 138.6-43.8c21.7-8.9 43-18.8 63.9-29.5V0H643.4c62.9 41.7 129.7 78.2 202.1 107.4C1020.4 178.1 1214.2 196.1 1397.5 154.8z'/%3E%3Cpath fill='%230D6CCD'  d='M1315.3 72.4c75.3-12.6 148.9-37.1 216.8-72.4h-723C966.8 71 1144.7 101 1315.3 72.4z'/%3E%3C/g%3E%3C/svg%3E");
		background-size: cover;
		text-align: center;
		height: 100%;
		overflow: hidden;
	}
	.bg-blur {
		position: absolute;
		margin: -140px auto auto -140px;
		left: 50%;
		top: 50%;
		width: 280px;
		height: 280px;
		border-radius: 50%;
		filter: blur(150px);
		-moz-opacity: 0.6;
		opacity: 0.6;
		background: #fff;
	}
	
	.login-box {
		position: absolute;
		margin: -180px auto auto -150px;
		left: 50%;
		top: 50%;
		width: 300px;
		-moz-opacity: 0.9;
		opacity: 0.9;
		.el-card__body {
			padding: 20px 20px 0;
		}
		.title {
			margin: -20px -20px 20px;
			height: 40px;
			line-height: 40px;
			color: #1898ca;
			font-size: 15px;
			font-weight: 500;
			text-shadow: 1px 1px #fff;
			box-shadow: 0 1px 4px rgba(0,0,0,0.08),0 0 60px rgba(0,0,0,0.01) inset;
		}
		.logo {
			margin-bottom: 20px;
			height: 40px;
			background: url(../../assets/images/logo.png) no-repeat center;
		}
		.el-input {
			height: 40px;
		}
		.el-button {
			display: block;
			width: 100%;
			height: 40px;
		}
		.code {
			width: 80px;
			height: 30px;
			vertical-align: middle;
			cursor: pointer;
		}
	}
</style>