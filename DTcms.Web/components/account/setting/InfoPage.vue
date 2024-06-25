<template>
	<common-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
		<el-tab-pane label="修改个人资料" name="info">
			<el-form-item label-width="0">
				<div style="display:flex;justify-content:center;width:100%;">
					<common-upload-avatar v-model="datas.model.avatar" title="上传头像" :boxwh="120" :width="300" :height="300" />
				</div>
			</el-form-item>
			<el-form-item label="真实姓名">
				<el-input v-model="datas.model.realName" placeholder="非必填，可空"></el-input>
			</el-form-item>
			<el-form-item prop="sex" label="性别">
				<el-radio-group v-model="datas.model.sex">
					<el-radio-button value="保密">保密</el-radio-button>
					<el-radio-button value="男">男</el-radio-button>
					<el-radio-button value="女">女</el-radio-button>
				</el-radio-group>
			</el-form-item>
			<el-form-item label="生日">
				<el-date-picker v-model="datas.model.birthday" type="date" value-format="YYYY-MM-DD" placeholder="选择日期"></el-date-picker>
			</el-form-item>
			<el-form-item label="注册时间">
				{{datas.model.regTime}}
			</el-form-item>
			<el-form-item label="最后登录时间">
				{{datas.model.lastTime}}
			</el-form-item>
			<el-form-item label="注册IP">
				{{datas.model.regIp}}
			</el-form-item>
			<el-form-item label="最后登录IP">
				{{datas.model.lastIp}}
			</el-form-item>
			<el-form-item>
				<el-button type="primary" :loading="datas.btnLoading" @click="submitForm">确认提交</el-button>
				<el-button @click="common.back(-1)">返回上一页</el-button>
			</el-form-item>
		</el-tab-pane>
	</common-form-box>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//页面SEO设置
	useSeoMeta({
		title: `编辑个人资料 - ${siteConfig.seoKeyword}`,
		ogTitle: siteConfig.seoKeyword,
		description: siteConfig.seoDescription,
		ogDescription: siteConfig.seoDescription,
	})
	//Ref对象
	const editFormRef = ref(null)
	//定义页面属性
	const datas = reactive({
		btnLoading: false,
		model: {
			avatar: null,
			realName: null,
			sex: null,
			birthday: null,
		},
		rules: {
			sex: [
				{ required: true, message: '请选择性别', trigger: 'change' },
			],
		}
	})
	
	//初始化数据
	const initData = async() => {
		datas.model = await useUser('info')
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				useHttp({
					method: 'put',
					url: `/account/member/info`,
					data: datas.model,
					successMsg: '修改资料已成功',
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
			}
		})
	}
	
	//页面完成后执行
	onMounted(async() => {
		initData()
	})
</script>