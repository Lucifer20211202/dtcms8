<template>
	<div class="mainbody">
		<dt-location :data="[{title:'系统功能'},{title:'编辑支付'}]"></dt-location>
		<div class="content-box">
			<dt-form-box ref="editFormRef" v-model="datas.model" :rules="datas.rules" activeName="info">
				<el-tab-pane label="基本信息" name="info">
					<div class="tab-content">
						<el-form-item prop="siteId" label="所属站点">
							<el-select v-model="datas.model.siteId" placeholder="请选择...">
								<el-option v-for="item in datas.siteList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="paymentId" label="支付平台">
							<el-select v-model="datas.model.paymentId" placeholder="请选择..." @change="handleTypeChange">
								<el-option v-for="item in datas.paymentList"
									:key="item.id"
									:label="item.title"
									:value="item.id">
								</el-option>
							</el-select>
						</el-form-item>
						<el-form-item v-if="datas.currPaymentType==2" prop="type" label="接口类型">
							<el-select v-model="datas.model.type" placeholder="请选择...">
								<el-option label="扫码支付接口" value="native"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item v-if="datas.currPaymentType==3" prop="type" label="接口类型">
							<el-select v-model="datas.model.type" placeholder="请选择...">
								<el-option label="PC网站支付接口" value="pc"></el-option>
							</el-select>
						</el-form-item>
						<el-form-item prop="title" label="支付名称">
							<el-input v-model="datas.model.title" placeholder="网站显示的标题，128字符内"></el-input>
						</el-form-item>
						<el-form-item label="AppID" v-if="datas.currPaymentType==2">
							<el-input v-model="datas.model.key1" placeholder="AppID(应用ID)"></el-input>
						</el-form-item>
						<el-form-item label="AppSecret" v-if="datas.currPaymentType==2">
							<el-input v-model="datas.model.key2" placeholder="AppSecret(应用密钥)"></el-input>
						</el-form-item>
						<el-form-item label="商户号" v-if="datas.currPaymentType==2">
							<el-input v-model="datas.model.key3" placeholder="微信支付的商户号"></el-input>
						</el-form-item>
						<el-form-item label="APIv3密钥" v-if="datas.currPaymentType==2">
							<el-input v-model="datas.model.key4" placeholder="微信商户APIv3密钥"></el-input>
						</el-form-item>
						<el-form-item label="CA证书路径" v-if="datas.currPaymentType==2">
							<el-input v-model="datas.model.key5" placeholder="根目录下的相对路径,如/cert/wechat.p12"></el-input>
						</el-form-item>
						<el-form-item label="支付宝AppID" v-if="datas.currPaymentType==3">
							<el-input v-model="datas.model.key1" placeholder="支付宝开放平台APPID"></el-input>
						</el-form-item>
						<el-form-item label="支付宝公钥" v-if="datas.currPaymentType==3">
							<el-input type="textarea" :rows="3" v-model="datas.model.key2" placeholder="普通公钥方式的支付宝公钥RSA2"></el-input>
						</el-form-item>
						<el-form-item label="应用私钥" v-if="datas.currPaymentType==3">
							<el-input type="textarea" :rows="3" v-model="datas.model.key3" placeholder="支付宝开放平台开发助手生成的应用私钥"></el-input>
						</el-form-item>
						<el-form-item prop="sortId" label="排序数字">
							<el-input v-model="datas.model.sortId" placeholder="数字越小越排前"></el-input>
						</el-form-item>
						<el-form-item label="创建时间" v-if="datas.model.id>0">
							{{datas.model.addBy}} {{datas.model.addTime}}
						</el-form-item>
					</div>
				</el-tab-pane>
			</dt-form-box>
		</div>
		
		<div class="footer-box">
			<div class="footer-btn">
				<el-button type="primary" :loading="datas.loading" @click="submitForm">确认保存</el-button>
				<el-button plain @click="$common.back()">返回上一页</el-button>
			</div>
		</div>
	</div>
</template>

<script setup>
	import { ref,reactive,getCurrentInstance } from "vue"
	import DtFormBox from '../../../components/layout/DtFormBox.vue'
	
	//获取全局属性
	const { proxy } = getCurrentInstance()
	//Ref对象
	const editFormRef = ref(null)
	
	//接收props传值
	const props = defineProps({
		id: 0
	})
	
	//定义页面属性
	const datas = reactive({
		loading: false,
		currPaymentType: 0,//当前收款类型
		siteList: [],
		paymentList: [],
		model: {
			id: 0,
			siteId: null,
			paymentId: null,
			type: null,
			title: null,
			key1: null,
			key2: null,
			key3: null,
			key4: null,
			key5: null,
			sortId: 99,
			addBy: null,
			addTime: null,
		},
		rules: {
			siteId: [
				{ required: true, message: '请选择所属站点', trigger: 'change' }
			],
			paymentId: [
				{ required: true, message: '请选择平台接入商', trigger: 'change' }
			],
			title: [
				{ required: true, message: '请输入支付方式名称', trigger: 'blur' }
			],
			type: [
				{ required: true, message: '请选择接口类型', trigger: 'change' }
			],
			sortId: [
				{ required: true, message: '请输入排序数字', trigger: 'blur' }
			]
		}
	})
	
	//初始化数据
	const initData = async() => {
		//加载支付平台列表
		await proxy.$api.request({
			url: '/admin/payment/view/0',
			success(res) {
				datas.paymentList = res.data
			}
		});
		//加载站点列表
		await proxy.$api.request({
			url: '/admin/site/view/0',
			success(res) {
				datas.siteList = res.data
			}
		})
		//修改时赋值
		if (props.id) {
			await proxy.$api.request({
				url: `/admin/site/payment/${props.id}`,
				loading: true,
				success(res) {
					//赋值给model
					datas.model = res.data
					//赋值当前类型
					handleTypeChange(datas.model.paymentId, false)
				}
			})
		}
	}
	//提交表单
	const submitForm = () => {
		//调用组件验证表单
		editFormRef.value.form.validate((valid) => {
			if(valid) {
				//Id大于0则修改，否则添加
				if (datas.model.id > 0) {
					proxy.$api.request({
						method: 'put',
						url: `/admin/site/payment/${datas.model.id}`,
						data: datas.model,
						successMsg: '修改支付方式已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							//可跳转加列表页
							proxy.$common.linkUrl('/setting/payment/list')
						},
						complete() {
							datas.loading = false
						}
					})
				} else {
					proxy.$api.request({
						method: 'post',
						url: '/admin/site/payment',
						data: datas.model,
						successMsg: '新增支付方式已成功',
						beforeSend() {
							datas.loading = true
						},
						success(res) {
							//重置表单
							editFormRef.value.form.resetFields()
						},
						complete() {
							datas.loading = false
						}
					})
				}
			}
		})
	}
	//选择收款类型回调
	const handleTypeChange = (val, check = true) => {
		datas.paymentList.forEach((item, index) => {
			if (item.id == val) {
				datas.currPaymentType = item.type
			}
		})
		if (check) {
			if (datas.currPaymentType == 0) {
				datas.model.type = "cash"
			} else if (datas.currPaymentType == 1) {
				datas.model.type = "balance"
			} else {
				datas.model.type = null
			}
		}
	}
	
	//运行初始化
	initData()
</script>