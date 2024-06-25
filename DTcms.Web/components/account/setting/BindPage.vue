<template>
	<el-tabs v-model="datas.activeName" @tab-change="handleTabChange" class="form-tabs">
		<el-tab-pane label="绑定手机" name="phone" class="form-box">
			<el-form ref="phoneFormRef" :model="datas.phoneModel" :rules="datas.rules" label-position="right" label-width="120px" @submit.native.prevent>
				<el-form-item label="当前手机号">
					<span v-if="userInfo.phone">{{userInfo.phone}}</span>
					<span v-else>您尚未绑定手机号码</span>
				</el-form-item>
				<el-form-item prop="phone" label="新手机号码">
					<el-input type="text" v-model="datas.phoneModel.phone" placeholder="请输入手机号码"></el-input>
				</el-form-item>
				<el-form-item prop="codeValue" label="验证码">
					<el-input type="text" v-model="datas.phoneModel.codeValue" placeholder="请输入手机验证码">
						<template #append>
							<el-button v-if="datas.timer>0">重新获取({{datas.timer}}s)</el-button>
							<el-button v-else @click="sendPhoneCode">获取验证码</el-button>
						</template>
					</el-input>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" :loading="datas.btnLoading" native-type="submit" @click="submitForm">确认提交</el-button>
					<el-button @click="common.back(-1)">返回上一页</el-button>
				</el-form-item>
			</el-form>
		</el-tab-pane>
		<el-tab-pane label="绑定邮箱" name="email" class="form-box">
			<el-form ref="emailFormRef" :model="datas.emailModel" :rules="datas.rules" label-position="right" label-width="120px" @submit.native.prevent>
				<el-form-item label="当前邮箱">
					<span v-if="userInfo.email">{{userInfo.email}}</span>
					<span v-else>您尚未绑定邮箱</span>
				</el-form-item>
				<el-form-item prop="email" label="新邮箱账号">
					<el-input type="text" v-model="datas.emailModel.email" placeholder="请输入新邮箱账号"></el-input>
				</el-form-item>
				<el-form-item prop="codeValue" label="验证码">
					<el-input type="text" v-model="datas.emailModel.codeValue" placeholder="请输入邮箱验证码">
						<template #append>
							<el-button v-if="datas.timer>0">重新获取({{datas.timer}}s)</el-button>
							<el-button v-else @click="sendEmailCode">获取验证码</el-button>
						</template>
					</el-input>
				</el-form-item>
				<el-form-item>
					<el-button type="primary" :loading="datas.btnLoading" native-type="submit" @click="submitForm">确认提交</el-button>
					<el-button @click="common.back(-1)">返回上一页</el-button>
				</el-form-item>
			</el-form>
		</el-tab-pane>
	</el-tabs>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `账号绑定管理 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	//获取当前用户信息
	const userInfo = await useUser('info')
	//Ref对象
	const phoneFormRef = ref(null)
	const emailFormRef = ref(null)
	//定义页面属性
	const datas = reactive({
		btnLoading: false,
		timer: 0, //计时器
		activeName: 'phone', //选项卡
		phoneModel: {
			phone: null,
			codeKey: null,
			codeValue: null,
		},
		emailModel: {
			email: null,
			codeKey: null,
			codeValue: null,
		},
		rules: {
			phone: [
				{ required: true, message: '请输入手机号码', trigger: 'blur' },
				{ pattern: /^1[0-9]{10}$/, message: '手机号码格式不正确', trigger: 'blur' },
			],
			email: [
				{ required: true, message: '请输入邮箱账户', trigger: 'blur' },
				{ pattern: /^(\w)+(\.\w+)*@(\w)+((\.\w+)+)$/, message: '邮箱格式有误', trigger: 'blur' },
			],
			codeValue: [
				{ required: true, message: '请输入验证码', trigger: 'blur' },
				{ min: 4, max: 8, message: '请输入 4 到 8 位的验证码', trigger: 'blur' },
			],
		}
	})
	
	//切换Tab事件
	const handleTabChange = () => {
		//清除其它表单验证
		if(datas.activeName == 'phone') {
			emailFormRef.value.clearValidate()
		} else {
			phoneFormRef.value.clearValidate()
		}
	}
	//获取手机验证码
	const sendPhoneCode = () => {
		if (!/(^1[3|4|5|7|8][0-9]{9}$)/.test(datas.phoneModel.phone)) {
			ElMessage.warning('提示，请输入正确的手机号码')
			return
		}
		//发送短信
		useHttp({
			url: `/verifycode/mobile/${datas.phoneModel.phone}?check=0`,
			success(res) {
				datas.timer = 120; //设置120秒
				countDown() //开始倒计时
				datas.phoneModel.codeKey = res.data.codeKey
			}
		})
	}
	//发送邮件验证码
	const sendEmailCode = () => {
		if (!/(^[_a-z0-9-]+(\.[_a-z0-9-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,})$)/.test(datas.emailModel.email)) {
			ElMessage.warning('提示，请输入正确的邮箱账户')
			return
		}
		//发送邮件
		useHttp({
			url: `/verifycode/email/${datas.emailModel.email}?check=0`,
			success(res) {
				datas.timer = 120 //设置120秒
				countDown() //开始倒计时
				datas.emailModel.codeKey = res.data.codeKey
			}
		})
	}
	//获取验证码计时器
	const countDown = () => {
		if (datas.timer > 0) {
			datas.timer--
			setTimeout(() => {
				countDown()
			}, 1000)
		}
	}
	//提交表单
	const submitForm = () => {
		let formObj = null
		let model = null
		let sendUrl = null
		//判断普通或验证码登录
		if(datas.activeName === 'phone') {
			formObj = phoneFormRef.value
			model = datas.phoneModel
			sendUrl = '/account/member/phone'
		} else {
			formObj = emailFormRef.value
			model = datas.emailModel
			sendUrl = '/account/member/email'
		}
		console.log(model)
		
		//表单验证
		formObj.validate((valid) => {
			if (valid) {
				useHttp({
					method: 'put',
					url: sendUrl,
					data: model,
					successMsg: '账号绑定成功',
					beforeSend() {
						datas.btnLoading = true
					},
					success(res) {
						//清空用户的state
						useUser('clear')
					},
					complete() {
						datas.btnLoading = false
					}
				})
			} else {
				return false
			}
		})
	}
	
</script>